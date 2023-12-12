using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class AppUsageStats
{

    public string packageName { get; set; }
    public long firstTimeStamp { get; set; }
    public long lastTimeStamp { get; set; }
    public long totalTimeInForeground { get; set; }

}
public class GameData
{
    public string userId;
    public AppUsageStats appUsageStats { get; set; }
    public int touchCounts { get; set; }
    public int accelerometerCount { get; set; }

}

public class RewardsResponse
{
    public long currentTimePlayed;
    public long currentSats;
    public long currentRequiredTime;
    public bool validated;
    public bool whitelisted;
    public bool blacklisted;
    public bool error;
    public string message;
}

public class SendToUsernameAPIResponse
{
    public string id;
    public string status;
    public string transactionId;
    public string receiverId;
    public string amount;
    public string comment;
    public string settledAt;
}


public class SendToUsernameResponse
{
    public bool success;
    public SendToUsernameAPIResponse data;
    public string message;
    public long currentTimePlayed;
    public long currentSats;
    public long currentRequiredTime;
}

public class SendToUsernamePayload
{
    public string gamertag;
    public string description;
    public string amount;

}



public class TokenPayloadExternal
{
    [JsonProperty("requestDetails")]
    public RequestDetails RequestDetails { get; set; }

    [JsonProperty("appIntegrity")]
    public AppIntegrity AppIntegrity { get; set; }

    [JsonProperty("deviceIntegrity")]
    public DeviceIntegrity DeviceIntegrity { get; set; }

    [JsonProperty("accountDetails")]
    public AccountDetails AccountDetails { get; set; }
}

public class RequestDetails
{
    [JsonProperty("requestPackageName")]
    public string RequestPackageName { get; set; }

    [JsonProperty("timestampMillis")]
    public string TimestampMillis { get; set; }

    [JsonProperty("requestHash")]
    public string RequestHash { get; set; }
}

public class AppIntegrity
{
    [JsonProperty("appRecognitionVerdict")]
    public string AppRecognitionVerdict { get; set; }

    [JsonProperty("packageName")]
    public string PackageName { get; set; }

    [JsonProperty("certificateSha256Digest")]
    public List<string> CertificateSha256Digest { get; set; }

    [JsonProperty("versionCode")]
    public string VersionCode { get; set; }
}

public class DeviceIntegrity
{
    [JsonProperty("deviceRecognitionVerdict")]
    public List<string> DeviceRecognitionVerdict { get; set; }
}

public class AccountDetails
{
    [JsonProperty("appLicensingVerdict")]
    public string AppLicensingVerdict { get; set; }
}

public class RootObject
{
    [JsonProperty("tokenPayloadExternal")]
    public TokenPayloadExternal TokenPayloadExternal { get; set; }
}

public class QuagoAccessTokenResponse
{
    [JsonProperty("AuthenticationResult")]
    public AuthenticationResult AuthenticationResult { get; set; }

    // If there are more properties inside "ChallengeParameters", you can define another class for it
    [JsonProperty("ChallengeParameters")]
    public object ChallengeParameters { get; set; }
}

public class PlayerInfo
{
    [JsonProperty("platforms")]
    public string Platforms { get; set; }

    [JsonProperty("last_seen_timestamp")]
    public DateTime LastSeenTimestamp { get; set; }

    [JsonProperty("cnt_inauth_days")]
    public int CountInauthDays { get; set; }

    [JsonProperty("last_inauth_timestamp")]
    public DateTime? LastInauthTimestamp { get; set; }  // nullable since it's "null" in the JSON

    [JsonProperty("total_playtime_hours")]
    public double TotalPlaytimeHours { get; set; }

    [JsonProperty("total_motion_hours")]
    public double? TotalMotionHours { get; set; }  // nullable since it's "null" in the JSON

    [JsonProperty("inauth_playtime_percentage")]
    public double InauthPlaytimePercentage { get; set; }

    [JsonProperty("emu_playtime_percentage")]
    public double EmuPlaytimePercentage { get; set; }

    [JsonProperty("emu_models")]
    public string EmuModels { get; set; }

    [JsonProperty("input_devices_found")]
    public string InputDevicesFound { get; set; }

    [JsonProperty("avg_session_playtime_minutes")]
    public double AvgSessionPlaytimeMinutes { get; set; }

    [JsonProperty("avg_daily_playtime_hours")]
    public double AvgDailyPlaytimeHours { get; set; }

    [JsonProperty("cnt_device_ids")]
    public int CountDeviceIds { get; set; }

    [JsonProperty("cnt_ips")]
    public int CountIps { get; set; }

    [JsonProperty("countries")]
    public string Countries { get; set; }

    [JsonProperty("device_model")]
    public string DeviceModel { get; set; }

    [JsonProperty("system_version")]
    public string SystemVersion { get; set; }

    [JsonProperty("success")]
    public bool success { get; set; }

    [JsonProperty("error")]
    public string error { get; set; }
}

public class AuthRequest
{
    public string AuthFlow { get; set; }
    public AuthParameters AuthParameters { get; set; }
    public string ClientId { get; set; }
}

public class AuthParameters
{
    public string USERNAME { get; set; }
    public string PASSWORD { get; set; }
}


public class AuthenticationResult
{
    [JsonProperty("AccessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("ExpiresIn")]
    public int ExpiresIn { get; set; }

    [JsonProperty("IdToken")]
    public string IdToken { get; set; }

    [JsonProperty("RefreshToken")]
    public string RefreshToken { get; set; }

    [JsonProperty("TokenType")]
    public string TokenType { get; set; }
}