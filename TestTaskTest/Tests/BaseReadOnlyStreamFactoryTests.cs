using NUnit.Framework;
using TestTask;
using System.Collections.Generic;
using System;
using System.IO;

namespace TestTaskTest
{
    /// <summary>
    /// Тесты для тестирования проверки фабрики обертки над потоками
    /// </summary>
    public class BaseReadOnlyStreamFactoryTests
    {
        private BaseReadOnlyStreamFactory _factory = new BaseReadOnlyStreamFactory();
        private string createdFileName = "./temp.txt";

        [OneTimeTearDown]
        public void TearDown()
        {
            if (File.Exists(createdFileName))
            {
                File.Delete(createdFileName);
            }
        }

        [Test]
        public void GetInputStream_WhiteSpacePath_Throws()
        {
            // ARRANGE
            string fileName = "   ";

            // ACT

            // ASERT
            Assert.Throws<ArgumentNullException>(() => _factory.GetInputStream(fileName));
        }

        [Test]
        public void GetInputStream_NonExistingFile_Throws()
        {
            // ARRANGE
            string fileName = "./a.txt";

            if (File.Exists(fileName))
            {
                Assert.Ignore("Для запуска теста необходимо убрать файл a.txt из папки проекта");
            }
            // ACT

            // ASERT
            Assert.Throws<FileNotFoundException>(() => _factory.GetInputStream(fileName));
        }

        [Test]
        public void GetInputStream_ExistingFile_CreatesInstance()
        {
            // ARRANGE
            try
            {
                FileStream test = File.Create(createdFileName);
                test.Close();
            }
            catch
            {
                Assert.Ignore($"Не удалось создать файл {createdFileName}, проверьте среду выполнения!");
            }

            // ACT
            IReadOnlyStream instance = _factory.GetInputStream(createdFileName);

            // ASERT
            try
            {
                Assert.That(instance != null);
            }
            catch
            {
                throw;
            }
            finally
            {
                instance.Dispose();
            }
        }
    }
}