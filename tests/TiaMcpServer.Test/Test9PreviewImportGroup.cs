using TiaMcpServer.ModelContextProtocol;

namespace TiaMcpServer.Test
{
    /// <summary>
    /// Tests for how PreviewImport decides that a groupPath names the group an object already lives in.
    /// These tests do not connect to TIA Portal and do not open a project.
    /// </summary>
    [TestClass]
    public class Test9PreviewImportGroup
    {
        [TestMethod]
        public void Test_910_IsSameGroup_AcceptsSingleSegmentWithoutSystemFolder()
        {
            Assert.IsTrue(McpServer.IsSameGroup("001_Main/0_Main_CallEnv", "001_Main"));
        }

        [TestMethod]
        public void Test_911_IsSameGroup_AcceptsNestedGroupWithoutSystemFolder()
        {
            Assert.IsTrue(McpServer.IsSameGroup("001_Main/Sub/Block_1", "001_Main/Sub"));
        }

        [TestMethod]
        public void Test_912_IsSameGroup_AcceptsLeadingSystemFolder()
        {
            Assert.IsTrue(McpServer.IsSameGroup("001_Main/0_Main_CallEnv", "Program blocks/001_Main"));
            Assert.IsTrue(McpServer.IsSameGroup("001_Main/Sub/Block_1", "Programmbausteine/001_Main/Sub"));
        }

        [TestMethod]
        public void Test_913_IsSameGroup_MatchesRootObjectsForEmptyGroupPath()
        {
            Assert.IsTrue(McpServer.IsSameGroup("Block_1", string.Empty));
        }

        [TestMethod]
        public void Test_914_IsSameGroup_RejectsADifferentGroup()
        {
            Assert.IsFalse(McpServer.IsSameGroup("001_Main/Block_1", "002_Other"));
            Assert.IsFalse(McpServer.IsSameGroup("001_Main/Block_1", string.Empty));
            Assert.IsFalse(McpServer.IsSameGroup("Block_1", "001_Main"));
        }

        [TestMethod]
        public void Test_915_IsSameGroup_IgnoresCase()
        {
            Assert.IsTrue(McpServer.IsSameGroup("001_Main/Block_1", "001_MAIN"));
        }
    }
}
