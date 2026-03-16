// Action: "Spacefight – Chat Forwarder"
//
// TRIGGER OPTION A (empfohlen):
//   Core → Commands → Command: "fight" (ohne !) → Prefix: !
//   Cooldown: 30s per User direkt in Streamerbot setzen
//   Variablen: %user% = Angreifer, %commandTarget% = Ziel
//
// TRIGGER OPTION B (Fallback falls Commands nicht funktionieren):
//   Twitch → Chat Message → Filter: "Message starts with !fight"
//   Cooldown: 30s per User direkt in Streamerbot setzen
//   Dann unten im Code "useCommandTarget = false" setzen
//
// Der Code unterstützt beide Varianten automatisch.

using System;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    public bool Execute()
    {
        string sfSession = CPH.GetGlobalVar<string>("gw_spacefight_session", false);
        CPH.LogInfo($"[SF Forwarder] sfSession={sfSession ?? "NULL"}");
        if (string.IsNullOrEmpty(sfSession))
        {
            CPH.LogInfo("[SF Forwarder] kein sfSession – Overlay nicht verbunden");
            return true;
        }

        // Attacker aus Twitch-Args
        string attacker = "";
        string defender = "";

        if (args.ContainsKey("user") && args["user"] != null)
            attacker = args["user"].ToString().Trim();
        else if (args.ContainsKey("userName") && args["userName"] != null)
            attacker = args["userName"].ToString().Trim();

        // Defender: erst commandTarget (Command-Trigger), dann Message parsen (Chat-Trigger)
        if (args.ContainsKey("commandTarget") && args["commandTarget"] != null)
        {
            defender = args["commandTarget"].ToString().Trim().TrimStart('@');
        }
        else if (args.ContainsKey("message") && args["message"] != null)
        {
            string msg = args["message"].ToString().Trim();
            // !fight @user oder !fight user
            if (msg.Length > 6)
            {
                defender = msg.Substring(6).Trim().TrimStart('@');
            }
        }
        else if (args.ContainsKey("rawMessage") && args["rawMessage"] != null)
        {
            string msg = args["rawMessage"].ToString().Trim();
            if (msg.Length > 6)
            {
                defender = msg.Substring(6).Trim().TrimStart('@');
            }
        }

        if (string.IsNullOrEmpty(attacker) || string.IsNullOrEmpty(defender)) return true;

        // Sanitieren: nur alphanumerisch + Unterstrich, max 25 Zeichen
        var sb1 = new System.Text.StringBuilder();
        foreach (char ch in attacker)
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '_')
                sb1.Append(ch);
        attacker = sb1.ToString();

        var sb2 = new System.Text.StringBuilder();
        foreach (char ch in defender)
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '_')
                sb2.Append(ch);
        defender = sb2.ToString();

        if (attacker.Length == 0 || attacker.Length > 25) return true;
        if (defender.Length == 0 || defender.Length > 25) return true;
        if (attacker.ToLower() == defender.ToLower()) return true; // kein Selbst-Fight

        var payload = new JObject
        {
            ["event"]    = "chat_msg",
            ["user"]     = attacker,
            ["message"]  = "!fight @" + defender,
            ["ts"]       = DateTime.UtcNow.ToString("o")
        };

        // Server-ID 0 versuchen, falls das nicht klappt auf 1 wechseln
        bool sent = CPH.WebsocketCustomServerBroadcast(payload.ToString(), sfSession, 0);
        CPH.LogInfo($"[SF Forwarder] {attacker} fights {defender}, sent={sent}, session={sfSession}");
        return true;
    }
}
