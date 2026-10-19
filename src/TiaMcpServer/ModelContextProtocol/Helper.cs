using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using System;
using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class Helper
    {
        public static List<Attribute> GetAttributeList(IEngineeringObject obj)
        {
            var attributes = new List<Attribute>();

            if (obj != null)
            {
                foreach (var attr in obj.GetAttributeInfos())
                {
                    // Some attributes throw instead of returning a value (e.g. 'WebserverDefaultHTMLPage'
                    // when 'PermitDataAccessViaWebApi' is enabled); one of them must not fail the whole list
                    object? value;
                    try
                    {
                        value = ToSerializableValue(obj.GetAttribute(attr.Name));
                    }
                    catch (Exception)
                    {
                        value = null;
                    }

                    attributes.Add(new Attribute
                    {
                        Name = attr.Name,
                        Value = value,
                        AccessMode = Enum.GetName(typeof(EngineeringAttributeAccessMode), attr.AccessMode)
                    });
                }
            }

            return attributes;
        }

        /// <summary>
        /// Openness returns engineering objects (DeviceItemAssociation, MultilingualText, FileInfo, ...)
        /// for some attributes; they reference each other and break the JSON serialization of the
        /// response. Simple values are kept, everything else is passed on as text.
        /// </summary>
        private static object? ToSerializableValue(object? value)
        {
            if (value is null)
            {
                return null;
            }

            var type = value.GetType();
            if (type.IsPrimitive || type.IsEnum || value is string || value is DateTime || value is decimal || value is TimeSpan)
            {
                return value;
            }

            return value.ToString();
        }

        /// <summary>
        /// First available translation of a MultilingualText (tag and constant comments), or
        /// null when the text has no items. Openness returns one item per project language.
        /// </summary>
        public static string? FirstText(MultilingualText? text)
        {
            if (text == null)
            {
                return null;
            }

            foreach (var item in text.Items)
            {
                return item.Text;
            }

            return null;
        }

        public static BlockGroupInfo BuildBlockHierarchy(PlcBlockGroup group, TiaMcpServer.Siemens.Portal? portal = null)
        {
            var groupInfo = new BlockGroupInfo
            {
                Name = group.Name
            };

            var blockList = new List<ResponseBlockInfo>();
            foreach (var block in group.Blocks)
            {
                var attributes = Helper.GetAttributeList(block);
                blockList.Add(new ResponseBlockInfo
                {
                    Path = portal == null ? null : portal.GetBlockPath(block),
                    Name = block.Name,
                    TypeName = block.GetType().Name,
                    Namespace = block.Namespace,
                    ProgrammingLanguage = Enum.GetName(typeof(ProgrammingLanguage), block.ProgrammingLanguage),
                    MemoryLayout = Enum.GetName(typeof(MemoryLayout), block.MemoryLayout),
                    IsConsistent = block.IsConsistent,
                    HeaderName = block.HeaderName,
                    ModifiedDate = block.ModifiedDate,
                    IsKnowHowProtected = block.IsKnowHowProtected,
                    Attributes = attributes,
                    Description = block.ToString()
                });
            }
            groupInfo.Blocks = blockList;

            var groupList = new List<BlockGroupInfo>();
            foreach (var subGroup in group.Groups)
            {
                groupList.Add(BuildBlockHierarchy(subGroup, portal));
            }
            groupInfo.Groups = groupList;

            return groupInfo;
        }
    }
}
