using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TestTask.Tests
{
    [TestFixture]
    public class ReadOnlyStreamShould
    {
        private IReadOnlyStream GenerateStreamFromString(string input)
        {
            return new ReadOnlyStream(new MemoryStream(Encoding.UTF8.GetBytes(input ?? "")));
        }
        
        /// <summary>
        ///     Тест на простое посимвольное чтение.
        /// </summary>
        /// <param name="input">Входная строка для инициализации потока</param>
        [TestCase("Lorem Ipsum - simply dummy text")]
        [TestCase("это текст-рыба")]
        [TestCase("ը տպագրության և տպագրական")]
        [TestCase("也称乱数假文或者哑元文本")]
        public void RegularRiding(string input)
        {
            var inputStream = GenerateStreamFromString(input);

            foreach (var expectedChar in input)
            {
                var actualChar = inputStream.ReadNextChar();
                Assert.AreEqual(expectedChar, actualChar);
            }
            
            inputStream.Close();
        }

        /// <summary>
        ///    Тест на корректную обработку чтения после конца файла.
        /// </summary>
        /// <param name="input">Входная строка для инициализации потока</param>
        [TestCase("1")]
        [TestCase("22")]
        [TestCase("333")]
        [TestCase("4444")]
        public void ThrowsExceptionOnReadAfterEof(string input)
        {
            var inputStream = GenerateStreamFromString(input);

            while (!inputStream.IsEof)
            {
                inputStream.ReadNextChar();
            }

            Assert.Throws<EndOfStreamException>(() => inputStream.ReadNextChar());
            inputStream.Close();
        }
        
        
        [TestCase("123", 0)]
        public void ThrowsExceptionOnReadAfterClose(string input, int positionToClose)
        {
            var inputStream = GenerateStreamFromString(input);

            for (var i = 0; i < positionToClose; i++)
            {
                inputStream.ReadNextChar();
            }
            inputStream.Close();

            Assert.Throws<ObjectDisposedException>(() => inputStream.ReadNextChar());
        }

        /// <summary>
        ///     Тест сброса позиции.
        /// </summary>
        /// <param name="input">Входная строка для инициализации потока</param>
        /// <param name="positionsToReset">Набор позиций, после которых производится сброс потока</param>
        /// <param name="expectedInput">Ожидаемый набор прочитанных символов, с учетом сбросов</param>
        /// <example>
        /// Пример данных на тест. Входная строка: 123; Позиции сброса: 1, 2
        ///     Шаг 1. Читаем первый символ и сбрасываем поток. Итого прочитано: 1(до 0)
        ///     Шаг 2. Читаем первые два символа и сбрасываем поток. Итого прочитано: 1(до 0) + 12(до 1)
        ///     Шаг 3. Позиций сброса больше нет. Строка читает до конца. Итого: 1(до 0) + 12(до 1) + 123
        ///     По итогу должен получиться набор 112123
        /// </example>
        [TestCase("123", new [] {0, 1}, "112123")]
        [TestCase("123", new [] {0}, "1123")]
        [TestCase("123", new [] {2}, "123123")]
        [TestCase("Lorem Ipsum", new int[] {}, "Lorem Ipsum")]
        public void ResetPosition(string input, IEnumerable<int> positionsToReset, string expectedInput)
        {
            var inputStream = GenerateStreamFromString(input);
            var streamPos = 0;
            var expectedChars = new Queue<char>(expectedInput.ToCharArray());
            var resetPositions = new Queue<int>(positionsToReset);
            
            while (!inputStream.IsEof)
            {
                var expected = expectedChars.Dequeue();
                var actual = inputStream.ReadNextChar();
                Assert.AreEqual(expected, actual);

                if (resetPositions.Any() && streamPos == resetPositions.Peek())
                {
                    resetPositions.Dequeue();
                    inputStream.ResetPositionToStart();
                    streamPos = 0;
                    continue;
                }
                
                streamPos++;
            }
            
            Assert.IsEmpty(expectedChars);
            inputStream.Close();
        }
    }
}
