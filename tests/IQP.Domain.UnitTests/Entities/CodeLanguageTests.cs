using IQP.Domain.Entities.AlgoTasks;
using IQP.Domain.Exceptions;

namespace IQP.Domain.UnitTests.Entities;

 [TestFixture]
    public class CodeLanguageTests
    {
        [Test]
        public void Can_Create_Valid_CodeLanguage()
        {
            // Arrange
            const string name = "CSharp";
            const string slug = "csharp";
            const string extension = ".cs";

            // Act
            var codeLanguage = CodeLanguage.Create(name, slug, extension);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(codeLanguage, Is.Not.Null);
                Assert.That(codeLanguage.Name, Is.EqualTo(name));
                Assert.That(codeLanguage.Slug, Is.EqualTo(slug));
                Assert.That(codeLanguage.Extension, Is.EqualTo(extension));
            });
        }
        
        [Test]
        public void CodeLanguage_Slug_Is_Created_In_Lowercase()
        {
            // Arrange
            const string name = "JavaScript";
            const string mixedCaseSlug = "JavaScript";
            const string extension = ".js";

            // Act
            var codeLanguage = CodeLanguage.Create(name, mixedCaseSlug, extension);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(codeLanguage.Slug, Is.EqualTo(mixedCaseSlug.ToLower()));
            });
        }
        
        [Test]
        public void Cannot_Create_CodeLanguage_With_Invalid_Slug()
        {
            // Arrange
            const string name = "CSharp";
            const string invalidSlug = "csharpexceedinglength";
            const string extension = ".cs";

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                CodeLanguage.Create(name, invalidSlug, extension));

            // Assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Errors["slug"][0], Is.EqualTo("Slug must be at most 10 characters long and not empty."));
        }

        [Test]
        public void Cannot_Create_CodeLanguage_With_Invalid_Extension_NoDot()
        {
            // Arrange
            const string name = "CSharp";
            const string slug = "csharp";
            const string invalidExtension = "cs";

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                CodeLanguage.Create(name, slug, invalidExtension));

            // Assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Errors["extension"][0], Is.EqualTo("Extension must be between 2 and 10 characters long and not empty. It must start with a dot and contain only lowercase letters."));
        }
        
        [Test]
        public void Cannot_Create_CodeLanguage_With_Invalid_Extension_NoLowerCase()
        {
            // Arrange
            const string name = "CSharp";
            const string slug = "csharp";
            const string invalidExtension = ".CS";

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                CodeLanguage.Create(name, slug, invalidExtension));

            // Assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Errors["extension"][0], Is.EqualTo("Extension must be between 2 and 10 characters long and not empty. It must start with a dot and contain only lowercase letters."));
        }

        [Test]
        public void Can_Update_CodeLanguage_With_Valid_Values()
        {
            // Arrange
            var codeLanguage = CodeLanguage.Create("CSharp", "cshar", ".cs");
            const string newName = "CSharp";
            const string newSlug = "csharp";
            const string newExtension = ".cs";

            // Act
            codeLanguage.Update(newName, newSlug, newExtension);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(codeLanguage.Name, Is.EqualTo(newName));
                Assert.That(codeLanguage.Slug, Is.EqualTo(newSlug));
                Assert.That(codeLanguage.Extension, Is.EqualTo(newExtension));
            });
        }

        [Test]
        public void Cannot_Update_CodeLanguage_With_Invalid_Slug()
        {
            // Arrange
            var codeLanguage = CodeLanguage.Create("CSharp", "csharp", ".cs");

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                codeLanguage.Update("CSharp", "csharpLongSlug", ".cs"));

            // Assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Errors["slug"][0], Is.EqualTo("Slug must be at most 10 characters long and not empty."));
        }
        
        [Test]
        public void CodeLanguage_Slug_Is_Updated_In_Lowercase()
        {
            // Arrange
            const string name = "JavaScript";
            const string badSlug = "avascript"; // Note: "javascript" is 10 characters long.
            const string newMixedCaseSlug = "JavaScript";
            const string extension = ".js";
            var codeLanguage = CodeLanguage.Create(name, badSlug, extension);

            // Act
            codeLanguage.Update(name, newMixedCaseSlug, extension);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(codeLanguage.Slug, Is.EqualTo(newMixedCaseSlug.ToLower()));
            });
        }

        [Test]
        public void Cannot_Update_CodeLanguage_With_Invalid_Extension()
        {
            // Arrange
            var codeLanguage = CodeLanguage.Create("CSharp", "csharp", ".cs");

            // Act
            var ex = Assert.Throws<ValidationException>(() =>
                codeLanguage.Update("CSharp", "csharp", "cs"));

            // Assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Errors["extension"][0], Is.EqualTo("Extension must be between 2 and 10 characters long and not empty. It must start with a dot and contain only lowercase letters."));
        }
    }