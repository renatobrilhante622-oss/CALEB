using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace BloxWorld
{
    [DefaultExecutionOrder(-1000)]
    public sealed class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public SaveData Data { get; private set; }
        public bool WriteBlocked { get; private set; }
        public event Action<string> SaveFailed;
        public string SavePath => Path.Combine(Application.persistentDataPath, "bloxworld.json");
        float nextSave;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Data = Load();
            nextSave = Time.unscaledTime + 5f;
        }

        SaveData Read(string path)
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith("{"))
                throw new InvalidDataException("Invalid save JSON.");
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data == null || data.version < 1) throw new InvalidDataException("Invalid save version.");
            if (data.version > 1)
            {
                WriteBlocked = true;
                throw new InvalidDataException("Save belongs to a newer game version. Writing disabled.");
            }
            data.Normalize();
            return data;
        }

        SaveData Load()
        {
            foreach (string path in new[] { SavePath, SavePath + ".bak" })
            {
                try
                {
                    var data = Read(path);
                    if (data != null) return data;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException)
                {
                    Debug.LogWarning($"Could not load {path}: {ex.Message}");
                    if (WriteBlocked) return new SaveData();
                    // Preserve unreadable files for recovery before creating a replacement.
                    try
                    {
                        File.Copy(path, path + ".corrupt-" + Guid.NewGuid().ToString("N"), false);
                    }
                    catch (Exception archiveError)
                    {
                        WriteBlocked = true;
                        Debug.LogError("Recovery copy failed; writing disabled: " + archiveError.Message);
                        return new SaveData();
                    }
                }
            }
            return new SaveData();
        }

        public bool SaveNow()
        {
            if (WriteBlocked || Data == null) return false;
            string temporary = SavePath + ".tmp";
            try
            {
                Data.Normalize();
                Directory.CreateDirectory(Application.persistentDataPath);
                byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(Data, true));
                using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(SavePath)) File.Replace(temporary, SavePath, SavePath + ".bak");
                else File.Move(temporary, SavePath);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is NotSupportedException)
            {
                Debug.LogError("Save failed: " + ex.Message);
                SaveFailed?.Invoke(ex.Message);
                return false;
            }
        }

        void Update()
        {
            if (Time.unscaledTime < nextSave) return;
            nextSave = Time.unscaledTime + 5f;
            SaveNow();
        }
        void OnApplicationPause(bool paused) { if (paused) SaveNow(); }
        void OnApplicationFocus(bool focused) { if (!focused) SaveNow(); }
        void OnApplicationQuit() { SaveNow(); }
        void OnDestroy() { if (Instance == this) Instance = null; }
    }
}
