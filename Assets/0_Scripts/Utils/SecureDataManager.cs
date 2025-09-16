// SecureDataManager.cs
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Steamworks;
using UnityEngine;

public class SecureDataManager
{
    // --- 암호화 키/IV 생성부 ---
    private static readonly string salt = "your_very_unique_and_secret_salt_string"; // 이 솔트 값을 직접 수정해서 사용하세요!
    private static byte[] _key;
    private static byte[] _iv;

    // Key 프로퍼티: _key가 비어있을 때만 GenerateKeyAndIV()를 호출
    private static byte[] Key
    {
        get
        {
            if (_key == null)
            {
                GenerateKeyAndIV();
            }
            return _key;
        }
    }

    // IV 프로퍼티: _iv가 비어있을 때만 GenerateKeyAndIV()를 호출
    private static byte[] IV
    {
        get
        {
            if (_iv == null)
            {
                GenerateKeyAndIV();
            }
            return _iv;
        }
    }

    // --- 파일 경로 ---
    private static string _savePath; // 값을 바로 할당하지 않음
    private static string SavePath // 프로퍼티(Property)로 변경
    {
        get
        {
            // _savePath가 비어있을 경우에만 딱 한 번 경로를 생성
            if (string.IsNullOrEmpty(_savePath))
            {
                _savePath = Path.Combine(Application.persistentDataPath, "playerData.sav");
            }
            return _savePath;
        }
    }

    private static void GenerateKeyAndIV()
    {
        // --- 대체 ID ---
        // 기기 ID를 가져오지 못할 경우를 대비한 고정 문자열입니다.
        // 고유 ID보다는 보안이 약하지만, 시스템이 항상 동작하도록 보장합니다.
        const string fallbackId = "a_static_fallback_id_for_editor_or_error";

        // string deviceId = SystemInfo.deviceUniqueIdentifier;
        string deviceId = SteamManager.Initialized ? SteamUser.GetSteamID().ToString() : fallbackId;

        // --- 안정성 강화 로직 ---
        // 기기 ID가 비어있거나, 유니티가 반환하는 기본 에러 값일 경우 대체 ID를 사용합니다.
        if (string.IsNullOrEmpty(deviceId) || deviceId != SteamUser.GetSteamID().ToString())
        {
            Debug.LogWarning($"<color=orange>[경고] 유효하지 않은 기기 ID가 감지되었습니다. 키 생성을 위해 대체 ID를 사용합니다. (에디터에서는 정상적인 동작일 수 있습니다.)</color>");
            deviceId = fallbackId;
        }

        string combinedString = deviceId + salt;

        using (var sha256 = SHA256.Create())
        {
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
        }
        using (var md5 = MD5.Create())
        {
            _iv = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
        }

        if (_key == null || _key.Length == 0)
        {
            Debug.LogError("<color=red>[심각한 오류] 키 생성에 완전히 실패했습니다!</color>");
        }
    }

    // --- 데이터 저장 함수 ---
    // 👇 Save 함수에 진단용 로그를 대폭 추가했습니다.
    public void Save(PlayerData data)
    {
        // --- 진단 1: 입력된 데이터가 null인지 확인 ---
        if (data == null)
        {
            Debug.LogError("<color=red>데이터 저장 실패: Save 함수에 전달된 PlayerData 객체가 null 입니다!</color>");
            return; // 데이터가 없으므로 저장 중단
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            // --- 진단 2: JSON 변환이 제대로 되었는지 확인 ---
            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                Debug.LogError("<color=red>데이터 저장 실패: PlayerData가 JSON으로 변환되지 않았습니다. PlayerData 클래스에 [System.Serializable] 속성이 올바르게 있는지, 모든 public 필드가 변환 가능한 타입인지 확인하세요.</color>");
                // 현재 데이터 내용을 강제로 출력해서 확인
                Debug.Log($"[진단] PlayerData 내용: Gold={data.gold}, Level={data.playerLevel}");
                return; // 변환 실패했으므로 저장 중단
            }

            // --- 진단 3: 성공 시 변환된 JSON 내용 확인 ---
            Debug.Log($"<color=yellow>[진단] 성공적으로 생성된 JSON:</color> {json}");

            byte[] encryptedData = Encrypt(json);
            File.WriteAllBytes(SavePath, encryptedData);
            Debug.Log($"<color=green>데이터 저장 성공:</color> {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>데이터 저장 실패 (Exception 발생):</color> {e.Message}\n{e.StackTrace}");
        }
    }

    // --- 데이터 불러오기 함수 ---
    public PlayerData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("저장 파일이 존재하지 않습니다. 새 데이터를 생성합니다.");
            PlayerData newData = new PlayerData();
            Save(newData); // 새 데이터를 바로 저장
            return newData;
        }

        try
        {
            byte[] encryptedData = File.ReadAllBytes(SavePath);
            // 데이터가 비어있는 경우 처리
            if (encryptedData == null || encryptedData.Length == 0)
            {
                throw new System.Exception("저장 파일의 내용이 비어있습니다.");
            }
            string json = Decrypt(encryptedData);
            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("<color=cyan>데이터 불러오기 성공!</color>");
            return loadedData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>데이터 불러오기 실패:</color> {e.Message}");
            Debug.LogWarning("기존 저장 파일을 삭제하고 새 데이터를 생성 및 저장합니다.");
            File.Delete(SavePath);
            PlayerData newData = new PlayerData();
            Save(newData); // 새 데이터를 생성하고 바로 저장
            return newData;
        }
    }

    // --- 암호화/복호화 로직 ---
    private byte[] Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return ms.ToArray();
            }
        }
    }

    private string Decrypt(byte[] cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream(cipherText))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}