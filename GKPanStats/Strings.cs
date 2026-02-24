using System.Collections.Generic;
using System.Globalization;

namespace GKPanStats
{
    public static class Strings
    {
        private static readonly Dictionary<string, Dictionary<string, string>> All = new()
        {
            ["en"] = new Dictionary<string, string>
            {
                ["cpu"] = "CPU", ["ram"] = "RAM", ["disk"] = "Disk",
                ["network"] = "Net", ["battery"] = "Battery", ["no_battery"] = "No Battery",
                ["loading"] = "Loading...", ["about"] = "About GKPanStats",
                ["quit"] = "Quit GKPanStats", ["created_by"] = "Created by",
                ["version"] = "Version", ["monitor"] = "Windows System Monitor"
            },
            ["el"] = new Dictionary<string, string>
            {
                ["cpu"] = "CPU", ["ram"] = "RAM", ["disk"] = "Δίσκος",
                ["network"] = "Δίκτυο", ["battery"] = "Μπαταρία", ["no_battery"] = "Χωρίς Μπαταρία",
                ["loading"] = "Φόρτωση...", ["about"] = "Σχετικά με το GKPanStats",
                ["quit"] = "Τερματισμός GKPanStats", ["created_by"] = "Δημιουργήθηκε από",
                ["version"] = "Έκδοση", ["monitor"] = "Παρακολούθηση Συστήματος Windows"
            },
            ["fr"] = new Dictionary<string, string>
            {
                ["cpu"] = "CPU", ["ram"] = "RAM", ["disk"] = "Disque",
                ["network"] = "Réseau", ["battery"] = "Batterie", ["no_battery"] = "Pas de batterie",
                ["loading"] = "Chargement...", ["about"] = "À propos de GKPanStats",
                ["quit"] = "Quitter GKPanStats", ["created_by"] = "Créé par",
                ["version"] = "Version", ["monitor"] = "Moniteur Système Windows"
            },
            ["it"] = new Dictionary<string, string>
            {
                ["cpu"] = "CPU", ["ram"] = "RAM", ["disk"] = "Disco",
                ["network"] = "Rete", ["battery"] = "Batteria", ["no_battery"] = "Nessuna batteria",
                ["loading"] = "Caricamento...", ["about"] = "Informazioni su GKPanStats",
                ["quit"] = "Esci da GKPanStats", ["created_by"] = "Creato da",
                ["version"] = "Versione", ["monitor"] = "Monitor di Sistema Windows"
            },
            ["de"] = new Dictionary<string, string>
            {
                ["cpu"] = "CPU", ["ram"] = "RAM", ["disk"] = "Festplatte",
                ["network"] = "Netzwerk", ["battery"] = "Batterie", ["no_battery"] = "Kein Akku",
                ["loading"] = "Laden...", ["about"] = "Über GKPanStats",
                ["quit"] = "GKPanStats beenden", ["created_by"] = "Erstellt von",
                ["version"] = "Version", ["monitor"] = "Windows Systemmonitor"
            }
        };

        public static Dictionary<string, string> GetStrings()
        {
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return All.ContainsKey(lang) ? All[lang] : All["en"];
        }
    }
}
