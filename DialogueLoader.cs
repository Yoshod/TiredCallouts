using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TiredCallouts
{
    public class DialogueLoader
    {
        private static DialogueLoader _instance;
        private readonly Dictionary<string, Dictionary<string, Dictionary<int, string>>> _dialogues;

        // Private constructor to enforce singleton pattern
        private DialogueLoader()
        {
            _dialogues = new Dictionary<string, Dictionary<string, Dictionary<int, string>>>();
            LoadDialogue();
        }

        // Singleton instance access
        public static DialogueLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DialogueLoader();
                }
                return _instance;
            }
        }

        // Load dialogue from XML
        private void LoadDialogue()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", "LSPDFR", "TiredCallouts", "Dialogue.xml");
            if (File.Exists(path))
            {
                XDocument doc = XDocument.Load(path);
                XElement root = doc.Element("Dialogues");

                if (root != null)
                {
                    foreach (XElement calloutElement in root.Elements())
                    {
                        string calloutName = calloutElement.Name.LocalName;
                        var calloutDict = new Dictionary<string, Dictionary<int, string>>();

                        foreach (XElement setElement in calloutElement.Elements())
                        {
                            string setId = setElement.Name.LocalName;
                            var lines = new Dictionary<int, string>();

                            foreach (XElement lineElement in setElement.Elements("Line"))
                            {
                                if (int.TryParse(lineElement.Attribute("id").Value, out int lineId))
                                {
                                    lines[lineId] = lineElement.Value;
                                }
                            }

                            foreach (XElement outcomeElement in setElement.Elements("Outcome"))
                            {
                                if (int.TryParse(outcomeElement.Attribute("id").Value, out int outcomeId))
                                {
                                    lines[outcomeId] = outcomeElement.Value;
                                }
                            }

                            calloutDict[setId] = lines;
                        }

                        _dialogues[calloutName] = calloutDict;
                    }
                }
                else
                {
                    throw new Exception("The expected root element 'Dialogues' was not found in the XML.");
                }
            }
            else
            {
                throw new FileNotFoundException($"Dialogue XML not found at path: {path}");
            }
        }

        // Method to retrieve a specific dialogue by Callout, Set, and Line/Outcome ID
        public string GetDialogue(string calloutName, string setId, int id)
        {
            if (_dialogues.ContainsKey(calloutName) &&
                _dialogues[calloutName].ContainsKey(setId) &&
                _dialogues[calloutName][setId].ContainsKey(id))
            {
                return _dialogues[calloutName][setId][id];
            }
            return "Dialogue not found.";
        }
    }
}
