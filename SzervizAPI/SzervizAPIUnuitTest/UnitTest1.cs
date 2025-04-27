using Microsoft.VisualStudio.TestTools.UnitTesting;
using SzervizAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SzervizAPI.Tests
{
    [TestClass]
    public class DolgozoTests
    {
        [TestMethod]
        public void Dolgozo_Properties_GetSetCorrectly()
        {
            // Arrange
            var dolgozo = new Dolgozo();
            int? testId = 1;
            string testNev = "John Doe";
            string testBeosztas = "Szerelő";
            string testLakcim = "123 Teszt Utca";
            string testTelefonszam = "06301234567";
            string testEmail = "john@email.com";
            string testSzemelyazonosito = "ABC123456";

            // Act
            dolgozo.Dolgozo_id = testId;
            dolgozo.Nev = testNev;
            dolgozo.Beosztas = testBeosztas;
            dolgozo.Lakcim = testLakcim;
            dolgozo.Telefonszam = testTelefonszam;
            dolgozo.Email_cim = testEmail;
            dolgozo.Szemelyazonosito_igazolvany_szam = testSzemelyazonosito;

            // Assert
            Assert.AreEqual(testId, dolgozo.Dolgozo_id);
            Assert.AreEqual(testNev, dolgozo.Nev);
            Assert.AreEqual(testBeosztas, dolgozo.Beosztas);
            Assert.AreEqual(testLakcim, dolgozo.Lakcim);
            Assert.AreEqual(testTelefonszam, dolgozo.Telefonszam);
            Assert.AreEqual(testEmail, dolgozo.Email_cim);
            Assert.AreEqual(testSzemelyazonosito, dolgozo.Szemelyazonosito_igazolvany_szam);
        }

        [TestMethod]
        public void Dolgozo_NullableId_WorksCorrectly()
        {
            // Arrange
            var dolgozo = new Dolgozo();

            // Act
            dolgozo.Dolgozo_id = null;

            // Assert
            Assert.IsNull(dolgozo.Dolgozo_id);
        }

        [TestMethod]
        public void Dolgozo_KeyAttribute_IsPresent()
        {
            // Arrange
            var propertyInfo = typeof(Dolgozo).GetProperty("Dolgozo_id");

            // Act
            var keyAttribute = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false);

            // Assert
            Assert.IsTrue(keyAttribute.Length > 0, "Key attribútum szükséges");
        }

        [TestMethod]
        public void Dolgozo_DefaultConstructor_CreatesEmptyObject()
        {
            // Arrange & Act
            var dolgozo = new Dolgozo();

            // Assert
            Assert.IsNull(dolgozo.Dolgozo_id);
            Assert.IsNull(dolgozo.Nev);
            Assert.IsNull(dolgozo.Beosztas);
            Assert.IsNull(dolgozo.Lakcim);
            Assert.IsNull(dolgozo.Telefonszam);
            Assert.IsNull(dolgozo.Email_cim);
            Assert.IsNull(dolgozo.Szemelyazonosito_igazolvany_szam);
        }
    }
}