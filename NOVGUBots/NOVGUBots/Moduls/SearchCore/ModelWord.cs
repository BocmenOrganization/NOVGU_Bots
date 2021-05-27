namespace SearchCore
{
    public class ModelWord
    {
        public string Word { get; init; }
        public float DegreeSimilarity { get; init; }

        public ModelWord(string Word, float DegreeSimilarity) => (this.Word, this.DegreeSimilarity) = (Word, DegreeSimilarity);

        public override string ToString() => $"{Word} необходимая степень сходства {DegreeSimilarity}";
    }
}
