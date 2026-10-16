using System.IO;
using System.Linq;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.Test
{
    /// <summary>
    /// Tests for the file names built from TIA Portal object names. TIA Portal allows names such
    /// as 'T_Data->HMI' or 'GTS_EQ_I/FLower' that are not valid file names. These tests do not
    /// connect to TIA Portal.
    /// </summary>
    [TestClass]
    public class Test9SafeFileName
    {
        [TestMethod]
        [DataRow("T_Data->HMI", "T_Data-_HMI")]
        [DataRow("GTS_EQ_I/FLower", "GTS_EQ_I_FLower")]
        [DataRow("a<b|c\"d", "a_b_c_d")]
        public void Safe_InvalidCharacters_AreReplaced(string name, string expected)
        {
            // Arrange / Act
            var result = FileNames.Safe(name);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("FC_Block_1")]
        [DataRow("Motor Control.v2")]
        public void Safe_ValidName_IsReturnedUnchanged(string name)
        {
            // Arrange / Act
            var result = FileNames.Safe(name);

            // Assert
            Assert.AreEqual(name, result);
        }

        [TestMethod]
        public void Safe_NonAsciiLetters_AreKept()
        {
            // Arrange: built from character codes so this file stays ASCII
            var name = new string(new[] { (char)0x00C4, '_', 'G', 'r', (char)0x00F6, (char)0x00DF, 'e' });

            // Act
            var result = FileNames.Safe(name);

            // Assert
            Assert.AreEqual(name, result);
        }

        [TestMethod]
        public void Safe_EveryInvalidFileNameCharacter_YieldsAUsablePath()
        {
            // Arrange
            foreach (var invalid in Path.GetInvalidFileNameChars())
            {
                var name = "Block" + invalid + "Name";

                // Act
                var safe = FileNames.Safe(name);
                var path = Path.Combine(@"C:\export", safe + ".xml");

                // Assert
                Assert.IsFalse(safe.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0, $"U+{(int)invalid:X4} is left in '{safe}'");
                Assert.AreEqual(@"C:\export", Path.GetDirectoryName(path), $"U+{(int)invalid:X4} must not add a folder level");
            }
        }

        [TestMethod]
        [DataRow("Name.", "Name_")]
        [DataRow("Name  ", "Name__")]
        [DataRow("Name. .", "Name___")]
        public void Safe_TrailingDotsAndSpaces_AreReplaced(string name, string expected)
        {
            // Arrange / Act
            var result = FileNames.Safe(name);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("CON", "_CON")]
        [DataRow("nul", "_nul")]
        [DataRow("COM1", "_COM1")]
        [DataRow("LPT9.txt", "_LPT9.txt")]
        [DataRow("CONSOLE", "CONSOLE")]
        public void Safe_ReservedDeviceName_IsPrefixed(string name, string expected)
        {
            // Arrange / Act
            var result = FileNames.Safe(name);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Safe_EmptyName_BecomesUnderscore(string? name)
        {
            // Arrange / Act
            var result = FileNames.Safe(name);

            // Assert
            Assert.AreEqual("_", result);
        }

        [TestMethod]
        [DataRow("T_Data->HMI")]
        [DataRow("GTS_EQ_I/FLower")]
        [DataRow("CON.")]
        public void Safe_IsIdempotent(string name)
        {
            // Arrange
            var once = FileNames.Safe(name);

            // Act
            var twice = FileNames.Safe(once);

            // Assert
            Assert.AreEqual(once, twice);
        }
    }
}
