using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class VerbalDictionary
{
    public Dictionary<string, List<string>> Translations { get; } = new();
    public string Name { get; set; }
    public string? FromSavePath { get; set; }

    public VerbalDictionary(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException("Name must not be null"); }
        Name = name.ToLower();
        Initialization();
    }
    public VerbalDictionary(string name, string filePath)
    {
        if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException("Name must not be null"); }
        Name = name.ToLower();
        if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentNullException("Path must not be null"); }
        FromSavePath = filePath;
        Initialization();
    }
    public void AddTranslation(string word, string translation)
    {
        if (string.IsNullOrWhiteSpace(word)) { throw new ArgumentNullException("The parameter (word) must not be null"); }
        if (string.IsNullOrWhiteSpace(translation)) { throw new ArgumentNullException("The parameter (translation) must not be null"); }
        word = word.ToLower();
        if (Translations.ContainsKey(word))
        {
            Translations[word].Add(translation);
        }
        else
        {
            Translations.Add(word, new List<string> { translation });
        }
    }
    public void AddTranslation(string word, params string[] translations)
    {
        if (string.IsNullOrWhiteSpace(word)) { throw new ArgumentNullException("The parameter (word) must not be null"); }
        if (translations == null) { throw new ArgumentNullException("The parameter (translations) must not be null"); }
        word = word.ToLower();
        List<string> translationsList = new();
        foreach (var translation in translations)
        {
            if (string.IsNullOrWhiteSpace(translation)) { throw new ArgumentNullException("The translation in translations must not be null"); }
            translationsList.Add(translation.ToLower());
        }
        translationsList = translationsList.Union(translationsList).ToList();
        if (Translations.ContainsKey(word))
        {
            Translations[word] = Translations[word].Union(translationsList).ToList();
        }
        else
        {
            Translations.Add(word, translationsList);
        }
    }
    public void Remove(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) { throw new ArgumentNullException("The parameter (word) must not be null"); }
        if (!(Translations.ContainsKey(word.ToLower()))) { throw new KeyNotFoundException("Key (word) doesn`t found"); }
        Translations.Remove(word.ToLower());
    }

    private void Initialization()
    {
        if (string.IsNullOrWhiteSpace(FromSavePath)) { FromSavePath = "dictionary_saves.json"; }
        if (!(File.Exists(FromSavePath)) && FromSavePath == "dictionary_saves.json") { return; }
        if (!(File.Exists(FromSavePath))) { throw new FileNotFoundException("File does`t exists"); }
        using (FileStream fs = new FileStream(FromSavePath, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(fs))
        {
            string json_data = reader.ReadToEnd();
            if (json_data == "") { return; }
            var translations = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json_data);
            if (translations == null) { throw new FormatException("Incorrect format or file damaged"); }
            foreach (var translation in translations)
            {
                if (!(string.IsNullOrWhiteSpace(translation.Key)) || translation.Value != null)
                {
                    Translations.Add(translation.Key, translation.Value);
                }
                else
                {
                    throw new FormatException("Incorrect format or file damaged");
                }
            }
        }
    }
    public void Preservation(string SavingPath = "dictionary_saves.json")
    {
        if (string.IsNullOrWhiteSpace(SavingPath)) { throw new ArgumentNullException("The path must not be null"); }
        using (FileStream fs = new FileStream(SavingPath, FileMode.Create))
        {
            if (Translations.Count <= 0)
            {
                return;
            }
            JsonSerializer.SerializeAsync(fs, Translations, new JsonSerializerOptions() { WriteIndented = true });
        }
    }

    public override string ToString() => string.Join("\n", Translations.Select(t => $"{t.Key} - {string.Join(", ", t.Value)}"));
}
