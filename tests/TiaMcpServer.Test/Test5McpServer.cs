using System;
using System.Linq;
using TiaMcpServer.ModelContextProtocol;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.Test
{
    [TestClass]
    [DoNotParallelize]
    public class Test5McpServer
    {
        private readonly bool _isInitialized = false;

        [TestInitialize]
        public void ClassInit()
        {
            if (!_isInitialized)
            {
                Engineering.TiaMajorVersion = Settings.TiaMajorVersion;
                Openness.Initialize(Engineering.TiaMajorVersion);
            }

            var response = McpServer.Connect();
        }

        [TestCleanup]
        public void ClassCleanup()
        {
            McpServer.Disconnect();
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        // [DataRow(Settings.Session1ProjectPath)]
        public void Test_500_McpServer_OpenCloseProject(string projectPath)
        {

            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.CloseProject();
            WriteMessage("McpServer.CloseProject()", response);

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        // [DataRow(Settings.Session1ProjectPath)]
        public void Test_501_McpServer_GetProjectTree(string projectPath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetProjectTree();
            WriteMessage("McpServer.GetProjectTree()", response);

            Console.WriteLine("Structure: " + response?.Tree);

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        // [DataRow(Settings.Session1ProjectPath)]
        public void Test_502_McpServer_GetState(string projectPath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetState();
            WriteMessage("McpServer.GetState()", response);

            Console.WriteLine("GetState: Project = '" + response?.Project + "'");
            Console.WriteLine("GetState: Session = '" + response?.Session + "'");
            Console.WriteLine("GetState: IsConnected = '" + response?.IsConnected + "'");

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        public void Test_503_McpServer_GetProjects()
        {

            McpServer.Connect();
            //McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetProjects();

            if (response == null)
            {
                Console.WriteLine("McpServer.GetProjects(): response is null");
            }
            else
            {
                WriteMessage($"McpServer.GetProjects()", response);
                
                if (response.Items != null)
                {
                    response.Items.ToList().ForEach(item =>
                    {
                        Console.WriteLine($"Project {item.Name}");
                        WriteAttributes(item);
                    });
                }
                else
                {
                    Console.WriteLine("McpServer.GetProjects(): no attributes found");
                }
            }

            //McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        // [DataRow(Settings.Session1ProjectPath)]
        public void Test_504_McpServer_GetDevices(string projectPath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetDevices();
            WriteMessage("McpServer.GetDevices()", response);

            Console.WriteLine("GetDevices:");
            response?.Items?.ToList().ForEach(item => Console.WriteLine($"- '{item}'"));

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        public void Test_510_McpServer_GetOpenLibraries(string projectPath)
        {
            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var response = McpServer.GetOpenLibraries();
            WriteMessage("McpServer.GetOpenLibraries()", response);

            var items = response?.Items?.ToList();
            items?.ForEach(item => Console.WriteLine($"- {item.Kind}: '{item.Name}' {item.Path} ({item.Attributes?.Count()} attributes)"));

            McpServer.CloseProject();

            Assert.IsNotNull(items, "McpServer returned no items");
            Assert.IsTrue(items.Any(item => item.Kind == "Project"), "Project library is missing");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        public void Test_511_McpServer_GetReferencedLibraries(string projectPath)
        {
            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var response = McpServer.GetReferencedLibraries();
            WriteMessage("McpServer.GetReferencedLibraries()", response);

            var items = response?.Items?.ToList();
            items?.ForEach(item => Console.WriteLine($"- '{item.Name}' ({item.OpenLibraryKind ?? "not open"}): {item.TypeVersionCount} type version(s)"));

            McpServer.CloseProject();

            Assert.IsNotNull(items, "McpServer returned no items");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        public void Test_512_McpServer_GetMarkedObjects_NoSession_Throws(string projectPath)
        {
            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            try
            {
                var ex = Assert.ThrowsExactly<global::ModelContextProtocol.McpException>(() => McpServer.GetMarkedObjects());
                StringAssert.Contains(ex.Message, "local session");
            }
            finally
            {
                McpServer.CloseProject();
            }
        }

        [TestMethod]
        [DataRow(Settings.Session1ProjectPath)]
        public void Test_513_McpServer_GetMarkedObjects(string sessionPath)
        {
            McpServer.Connect();
            McpServer.OpenProject(sessionPath);

            var response = McpServer.GetMarkedObjects();
            WriteMessage("McpServer.GetMarkedObjects()", response);

            var items = response?.Items?.ToList();
            items?.ForEach(item => Console.WriteLine($"- {item.Kind}: '{item.Name}' {item.Path} (me={item.IsMarkedByMe}, others={item.IsMarkedByOthers}, upToDate={item.IsUpToDate})"));

            var markedByMe = McpServer.GetMarkedObjects("me").Items?.ToList();
            var markedByOthers = McpServer.GetMarkedObjects("others").Items?.ToList();

            McpServer.CloseProject();

            Assert.IsNotNull(items, "McpServer returned no items");
            CollectionAssert.AreEqual(items.Select(item => item.Name ?? "").ToList(), response!.Names!.ToList(), "Names must match the item names");
            Assert.IsTrue(markedByMe!.All(item => item.IsMarkedByMe), "filter 'me' returned an object not marked by me");
            Assert.IsTrue(markedByOthers!.All(item => item.IsMarkedByOthers), "filter 'others' returned an object not marked by others");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        public void Test_514_McpServer_CheckIsUpToDate_NoSession_Throws(string projectPath)
        {
            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            try
            {
                var ex = Assert.ThrowsExactly<global::ModelContextProtocol.McpException>(() => McpServer.CheckIsUpToDate());
                StringAssert.Contains(ex.Message, "local session");
            }
            finally
            {
                McpServer.CloseProject();
            }
        }

        [TestMethod]
        [DataRow(Settings.Session1ProjectPath)]
        public void Test_515_McpServer_CheckIsUpToDate(string sessionPath)
        {
            McpServer.Connect();
            McpServer.OpenProject(sessionPath);

            try
            {
                var response = McpServer.CheckIsUpToDate();
                WriteMessage("McpServer.CheckIsUpToDate()", response);

                Assert.IsNotNull(response);
            }
            finally
            {
                McpServer.CloseProject();
            }
        }

        [TestMethod]
        public void Test_516_McpServer_CommitChanges_WriteDisabled_Throws()
        {
            var previous = TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite;
            TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = false;

            try
            {
                var ex = Assert.ThrowsExactly<global::ModelContextProtocol.McpException>(() => McpServer.CommitChanges("comment"));
                StringAssert.Contains(ex.Message, "--allow-write");
            }
            finally
            {
                TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = previous;
            }
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath)]
        public void Test_517_McpServer_CommitChanges_NoSession_Throws(string projectPath)
        {
            var previous = TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite;
            TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = true;

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            try
            {
                var ex = Assert.ThrowsExactly<global::ModelContextProtocol.McpException>(() => McpServer.CommitChanges("comment"));
                StringAssert.Contains(ex.Message, "local session");
            }
            finally
            {
                McpServer.CloseProject();
                TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = previous;
            }
        }

        [TestMethod]
        [DataRow(Settings.Session1ProjectPath)]
        public void Test_518_McpServer_CommitChanges_EmptyComment_Throws(string sessionPath)
        {
            var previous = TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite;
            TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = true;

            McpServer.Connect();
            McpServer.OpenProject(sessionPath);

            try
            {
                var ex = Assert.ThrowsExactly<global::ModelContextProtocol.McpException>(() => McpServer.CommitChanges("  "));
                StringAssert.Contains(ex.Message, "comment");
            }
            finally
            {
                McpServer.CloseProject();
                TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = previous;
            }
        }

        [TestMethod]
        [Ignore("Ignored test: Creates a new revision on the multiuser server and closes the session: run manually against a disposable server project with Settings.Session1ProjectPath")]
        [DataRow(Settings.Session1ProjectPath)]
        public void Test_519_McpServer_CommitChanges(string sessionPath)
        {
            var previous = TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite;
            TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = true;

            try
            {
                McpServer.Connect();
                McpServer.OpenProject(sessionPath);

                Assert.IsTrue(McpServer.CheckIsUpToDate().IsUpToDate, "session must be up to date before the commit");

                var response = McpServer.CommitChanges("MCP test commit");
                WriteMessage("McpServer.CommitChanges()", response);

                Assert.IsTrue(response.Revision > 0, "commit must report the new revision");
            }
            finally
            {
                TiaMcpServer.ModelContextProtocol.WritePolicy.AllowWrite = previous;
            }
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath, Settings.Project1PlcSoftwarePath0)]
        //[DataRow(Settings.Session1ProjectPath, Settings.Session1PlcSoftwarePath)]
        public void Test_505_McpServer_GetSoftwareInfo(string projectPath, string softwarePath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetSoftwareInfo(softwarePath);
            WriteMessage("McpServer.GetSoftwareInfo()", response);

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath, "HMI_0")]
        //[DataRow(Settings.Session1ProjectPath, "PC-System_1")]
        public void Test_506_McpServer_GetDeviceInfo(string projectPath, string devicePath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetDeviceInfo(devicePath);
            WriteMessage("McpServer.GetDeviceInfo()", response);

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        [TestMethod]
        [DataRow(Settings.Project1ProjectPath, "PC-System_0/PLC_0")]
        //[DataRow(Settings.Session1ProjectPath, "PC-System_1/Software PLC_1")]
        public void Test_507_McpServer_GetDeviceItemInfo(string projectPath, string deviceItemPath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetDeviceItemInfo(deviceItemPath);
            WriteMessage("McpServer.GetDeviceItemInfo()", response);

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }


        [TestMethod]
        [DataRow(Settings.Project1ProjectPath, Settings.Project1PlcSoftwarePath0, "0_OBs/Main_1")]
        //[DataRow(Settings.Session1ProjectPath, Settings.Session1PlcSoftwarePath, "0_OBs/Main_1")]
        public void Test_508_McpServer_GetBlockInfo(string projectPath, string softwarePath, string blockPath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetBlockInfo(softwarePath, blockPath);
            WriteMessage("McpServer.GetBlockInfo()", response);

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }


        [TestMethod]
        [DataRow(Settings.Project1ProjectPath, Settings.Project1PlcSoftwarePath0, "Common/CarrierRegister/ML_SubstratState")]
        public void Test_509_McpServer_GetTypeInfo(string projectPath, string softwarePath, string blockPath)
        {

            McpServer.Connect();
            McpServer.OpenProject(projectPath);

            var success = true;
            var response = McpServer.GetTypeInfo(softwarePath, blockPath);
            WriteMessage("McpServer.GetTypeInfo()", response);

            McpServer.CloseProject();

            Assert.IsTrue(success, "McpServer failed");
        }

        private static void WriteMessage(string method, ResponseMessage? rm)
        {
            if (rm == null)
            {
                Console.WriteLine($"{method}: content is null");
            }
            else
            {

                Console.WriteLine($"{method}: {rm.Message}");
            }
        }

        private static void WriteAttributes(ResponseAttributes? ra)
        {
            if (ra != null && ra.Attributes != null)
            {
                foreach (var attribute in ra.Attributes)
                {
                    Console.WriteLine($"- {attribute.Name}: {attribute.Value}");
                }
            }
            else
            {
                Console.WriteLine($"no attributes found");
            }
        }


    }
}
