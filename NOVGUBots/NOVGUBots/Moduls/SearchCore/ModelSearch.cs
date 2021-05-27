using System;
using System.Linq;

namespace SearchCore
{
    public class ModelSearch
    {
        private ModelWord[][] searchStr;

        public ModelSearch(ModelWord[][] searchStr) => this.searchStr = searchStr;
        public ModelSearch(string[] words, bool autoCorrectKoof = true)
        {
            if (words == null || words.Length == 0)
                throw new ArgumentException("words - неверное значение аргумента");
            if (autoCorrectKoof)
            {
                int maxLength = words.Max(x => x.Length);
                searchStr = words.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new ModelWord[] { new(x, (float)x.Length / maxLength) }).ToArray();
            }
            else
                searchStr = words.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new ModelWord[] { new(x, 1f) }).ToArray();
        }
        /// <summary>
        /// Получение всех данных о сходстве, во 2 измерении содержатся слово/синонимы отсортированные по уменьшению степени сходства
        /// </summary>
        public DataSearch[][] GetDataArrayDegreeSimilarity(string[] words)
        {
            if (words == null || words.Length == 0) return null;
            return searchStr.Select(x =>
            {
                return x.Select(y =>
                {
                    ModelWord wordSearch = null;
                    float degreeSimilarity = -1;
                    string word = null;
                    foreach (var w in words)
                    {
                        int charCountIn = LevenshteinDistance(w, y.Word);
                        float degreeSimilarityWord = charCountIn == 0 ? 1 : (1 - (float)(charCountIn) / Math.Max(w.Length, y.Word.Length));
                        if (degreeSimilarityWord > degreeSimilarity)
                        {
                            word = w;
                            degreeSimilarity = degreeSimilarityWord;
                            wordSearch = y;
                        }
                    }
                    return new DataSearch(wordSearch, degreeSimilarity, word);
                }).OrderByDescending(x => x.DegreeSimilarity).ToArray();
            }).ToArray();
        }
        public DataSearch[] GetDataDegreeSimilarity(string[] words)
        {
            if (words == null || words.Length == 0) return null;
            var data = GetDataArrayDegreeSimilarity(words).Select(x => x.ToList()).Select((x, i) => (x, i, x.Sum(s => s.WordSearch.DegreeSimilarity) / x.Count)).ToList();
            // Проходим по словам
            foreach (var elem in words)
            {
                // Находим елементы где данное слово имеет наибольший коэфф
                var s = data.Where(x => x.x.Count > 0 && x.x[0].Word == elem).ToList();
                if (s.Count > 0)
                {
                    float maxDegreeSimilarity = -1;
                    int indexElemMaxDegreeSimilarity = -1;
                    // Среди элементов находим тот у которого самый высокий коэфф
                    for (int i = 0; i < s.Count; i++)
                    {
                        if (maxDegreeSimilarity < s[i].x[0].DegreeSimilarity)
                        {
                            indexElemMaxDegreeSimilarity = i;
                            maxDegreeSimilarity = (float)s[i].x[0].DegreeSimilarity;
                        }
                    }
                    // TODO у других удаляем данное слово
                    for (int i = 0; i < s.Count; i++)
                    {
                        if (i != indexElemMaxDegreeSimilarity)
                        {
                            if (data[s[i].i].x.Count == 1)
                                data[s[i].i].x.Add(new DataSearch(new ModelWord(null, data[s[i].i].Item3), null, null));
                            data[s[i].i].x.RemoveAt(0);
                        }
                    }
                }
                //else
                //    data.Add((new System.Collections.Generic.List<DataSearch> { new DataSearch(null, null, elem) }, data.Count));
            }
            return data.Select(x => x.x[0]).ToArray();  //data.Select(x => (x.x.Count > 0) ? (x.x[0].DegreeSimilarity > 0 ? x.x[0] : new DataSearch(null, null, x.x[0].Word)) : null).ToArray();
        }
        public float GetDegreeSimilarity(string[] words) => (words!= null && words.Length > 0) ? GetDegreeSimilarity(GetDataDegreeSimilarity(words)) : -1;
        public float GetDegreeSimilarity(DataSearch[] dataSearch) => dataSearch.Sum(x => (x?.DegreeSimilarity ?? 0) * (x?.WordSearch?.DegreeSimilarity ?? 1)) / dataSearch.Sum(x => x?.WordSearch.DegreeSimilarity ?? 1);

        private static int LevenshteinDistance(string string1, string string2)
        {
            int diff;
            int[,] m = new int[string1.Length + 1, string2.Length + 1];

            for (int i = 0; i <= string1.Length; i++) { m[i, 0] = i; }
            for (int j = 0; j <= string2.Length; j++) { m[0, j] = j; }

            for (int i = 1; i <= string1.Length; i++)
            {
                for (int j = 1; j <= string2.Length; j++)
                {
                    diff = (string1[i - 1] == string2[j - 1]) ? 0 : 1;

                    m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1, m[i, j - 1] + 1), m[i - 1, j - 1] + diff);
                }
            }
            return m[string1.Length, string2.Length];
        }
        public class DataSearch
        {
            public ModelWord WordSearch { get; init; }
            public float? DegreeSimilarity { get; init; }
            public string Word { get; init; }
            public DataSearch(ModelWord WordSearch, float? DegreeSimilarity, string Word) => (this.WordSearch, this.DegreeSimilarity, this.Word) = (WordSearch, DegreeSimilarity, Word);

            public override string ToString() => $"{Word} сходится на: {DegreeSimilarity} со словом {WordSearch}";
        }
    }
}