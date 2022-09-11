namespace OfferExporter.Tests
{
    public class OutputTests
    {
        readonly string solutionDirPath;
        public OutputTests()
        {
            solutionDirPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.ToString();
        }
        
        [Theory]
        [InlineData("Fnac.json.gz.md5")]
        [InlineData("Marketplace.json.gz.md5")]
        public void CompareMD5FilesToExpected(string fileName)
        {
            string outputFilePath = Path.Combine(solutionDirPath + @"\OfferExporter\bin\Debug\net6.0\_OUPUT", fileName);
            string expectedFilePath = Path.Combine(solutionDirPath + @"\OfferExporter\_ExpectedExports", fileName);

            string outputFile = File.ReadAllText(outputFilePath);
            string expectedFile = File.ReadAllText(expectedFilePath);

            Assert.Equal(expectedFile, outputFile);
        }
    }
}