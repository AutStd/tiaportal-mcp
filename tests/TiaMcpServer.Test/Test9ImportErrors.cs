using System;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.Test
{
    /// <summary>
    /// Tests for the message built from a failed import. TIA Portal names the exact problem in
    /// the exception, and the import tools used to drop it. These tests do not connect to TIA Portal.
    /// </summary>
    [TestClass]
    public class Test9ImportErrors
    {
        private const string FlowOrderMessage = "The elements must be sorted according to the current flow (UId 42)";

        [TestMethod]
        public void Describe_SingleException_ReturnsItsMessage()
        {
            // Arrange
            var exception = new InvalidOperationException("  Block is inconsistent  ");

            // Act
            var text = ImportErrors.Describe(exception);

            // Assert
            Assert.AreEqual("Block is inconsistent", text);
        }

        [TestMethod]
        public void Describe_InnerExceptions_AreJoinedOutermostFirst()
        {
            // Arrange
            var exception = new Exception("Import failed", new Exception("Network 2 is invalid", new Exception("Part 7 is unknown")));

            // Act
            var text = ImportErrors.Describe(exception);

            // Assert
            Assert.AreEqual("Import failed: Network 2 is invalid: Part 7 is unknown", text);
        }

        [TestMethod]
        public void Describe_RepeatedMessages_AppearOnce()
        {
            // Arrange
            var exception = new Exception("Same text", new Exception("Same text"));

            // Act
            var text = ImportErrors.Describe(exception);

            // Assert
            Assert.AreEqual("Same text", text);
        }

        [TestMethod]
        [DataRow(FlowOrderMessage)]
        [DataRow("THE ELEMENTS MUST BE SORTED ACCORDING TO THE CURRENT FLOW")]
        public void Describe_FlowOrderFailure_KeepsTiaMessageAndAddsHint(string tiaMessage)
        {
            // Arrange
            var exception = new Exception("Import failed", new Exception(tiaMessage));

            // Act
            var text = ImportErrors.Describe(exception);

            // Assert
            StringAssert.Contains(text, tiaMessage);
            StringAssert.Contains(text, "directly after the part that feeds it");
        }

        [TestMethod]
        public void Describe_OtherFailure_AddsNoHint()
        {
            // Arrange
            var exception = new Exception("Import failed", new Exception("Name 'FB_1' is already used"));

            // Act
            var text = ImportErrors.Describe(exception);

            // Assert
            Assert.IsFalse(text.Contains("flow order"), "The hint belongs to the flow-order failure only");
        }
    }
}
