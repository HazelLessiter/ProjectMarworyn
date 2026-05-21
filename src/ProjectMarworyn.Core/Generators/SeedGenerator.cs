using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Services;
using System.Security.Cryptography;
using System.Text;

namespace ProjectMarworyn.Core.Generators
{
    internal class SeedGenerator : ISeedGenerator
    {
        private readonly IFileManager _fileManager;
        private readonly IConsoleService _consoleService;

        public SeedGenerator(IFileManager fileManager,
            IConsoleService consoleService)
        {
            _fileManager = fileManager;
            _consoleService = consoleService;
        }

        public List<SeedWord> GetThreeWords()
        {
            var threeWordSeed = new List<SeedWord>();

            var seedWords = _fileManager.ReadSeedWordFile();

            if (seedWords == null ||
                seedWords.Count == 0)
            {
                return threeWordSeed;
            }

            for (int i = 0; i < 3; i++)
            {
                var randomNumber = RandomNumberGenerator.GetInt32(seedWords.Count);
                var randomSeedWord = seedWords[randomNumber];
                threeWordSeed.Add(randomSeedWord);
            }

            return threeWordSeed;
        }

        public int CreateWorldSeed(List<SeedWord> seedWords)
        {
            var worldSeed = 0;
            var threeWordSeed = "threeWorldSeed";

            if (seedWords.Count == 3)
            {
                threeWordSeed = $"{seedWords[0].Word
                    .ToUpper()}-{seedWords[1].Word
                    .ToUpper()}-{seedWords[2].Word
                    .ToUpper()}";

                worldSeed = BitConverter.ToInt32(SHA256
                        .HashData(Encoding.UTF8
                        .GetBytes(threeWordSeed)),
                    0);
            }

            _consoleService.WriteLine($"World seed created: {threeWordSeed}");

            return worldSeed;
        }
    }
}