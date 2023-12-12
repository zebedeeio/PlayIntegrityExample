using System;
using System.Collections;
using System.Collections.Generic;
using Beamable;
using Beamable.Server.Clients;
using Google.Play.Integrity;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using static ZBD.Models;
using Beamable.Common;

namespace ZBD
{
    public class RewardsController : MonoBehaviour
    {
        public bool debug;
        public Text satsLabel;
        public Text satsConvertedLabel;
        public long cloudProjectNumber;
        public TMP_Text countdownLabel;
        public TMP_Text responseLabel;
        public TMP_Text debugModeLabel;
        int checkTime = 60;
        private float currentTime;
        public String appToken;
        string deviceId;
        public RewardsResponse currentStats;

        public GameObject infoPanel;
        public GameObject successPanel;
        public GameObject errorPanel;
        public Text errorPanelText;

        public GameObject appUsagePanel;
        public GameObject withdrawPanel;
        public Text withdrawPanelSats;

        public Slider rewardsSlider;
        public Text rewardsSliderInfo;

        public InputField usernameField;

        public GameObject testButton;

        public GameObject withdrawButton;
        string beamableGamerTag;

        public Text beamableGamerTagLabel;
        Coroutine checkLoop;
        private void Awake()
        {
            if (!debug)
            {
                countdownLabel.gameObject.SetActive(false);
                responseLabel.gameObject.SetActive(false);
                testButton.gameObject.SetActive(false);
                debugModeLabel.gameObject.SetActive(false);
            }
        }

        public void ShowInfo()
        {
            infoPanel.SetActive(true);
        }

        public void CloseInfo()
        {
            infoPanel.SetActive(false);
        }
        void Start()
        {


            if (cloudProjectNumber == 0)
            {
                Debug.LogError("please set the google cloud project number in the inspector");
                return;
            }


            StartQuago();

            countdownLabel.text = "";


            if (!Utils.Instance.HasUsageStatsPermissionPage())
            {

                appUsagePanel.SetActive(true);

            }


            Invoke("CheckTimePlayed", 5);

            GetStats();

            usernameField.text = PlayerPrefs.GetString("username", "");

            GetBeamableGamerTag();
        }

        void StartQuago()
        {
            /*
             If using Quago for cheat detection make sure the appToken has been set
             If not you can remove this function
            */
            if (appToken.Length == 0)
            {
                Debug.LogError("app token not set in inspector, if you are not using Quago for anti cheat, remove StartQuago function");
                return;
            }
            Quago.initialize(QuagoSettings.create(appToken, QuagoSettings.QuagoFlavor.PRODUCTION).setLogLevel(QuagoSettings.LogLevel.INFO)
);
            deviceId = SystemInfo.deviceUniqueIdentifier;
            Quago.beginTracking(deviceId);
        }
        public async void GetBeamableGamerTag()
        {
            try
            {
                var ctx = BeamContext.Default;
                await ctx.OnReady;
                beamableGamerTag = ctx.Api.User.id + "";
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                beamableGamerTag = "error getting beamable id";
            }
            beamableGamerTagLabel.text = "id: " + beamableGamerTag;

        }

        public void Test()
        {
            currentTime = 0;
            CheckTimePlayed();
        }

        public void RequestAppUsage()
        {
            if (!Utils.Instance.HasUsageStatsPermissionPage())
            {

                Utils.Instance.OpenUsageStatsPermissionPage();
            }
            else
            {
                appUsagePanel.SetActive(false);
            }
        }
        public void ShowEarnings()
        {
            if (currentStats != null && currentStats.blacklisted)
            {
                ShowError("We need to verify your account first\nPlease contact support with id:" + beamableGamerTag);
                return;
            }

            if (!Utils.Instance.HasUsageStatsPermissionPage())
            {
                appUsagePanel.SetActive(true);


            }
            else
            {
                withdrawPanel.SetActive(true);
            }

        }

        void ResetTimePlayed()
        {
            Utils.Instance.ResetTouch();
            currentTime = checkTime;
            if (checkLoop != null)
            {
                StopCoroutine(checkLoop);
            }
            checkLoop = StartCoroutine(CountdownRoutine());
        }

        void ShowError(string message)
        {
            errorPanel.SetActive(true);
            errorPanelText.text = message;
        }



        void ShowSuccess()
        {
            successPanel.SetActive(true);
        }



        void CheckTimePlayed()
        {
            countdownLabel.text = "updating...";

            if (!Utils.Instance.HasUsageStatsPermissionPage())
            {
                countdownLabel.text = "please enabled app usage stats";
                ResetTimePlayed();
                return;
            }

            try
            {
                string usageStats = Utils.Instance.GetAndroidUsageStats();

                List<AppUsageStats> statsList = JsonConvert.DeserializeObject<List<AppUsageStats>>(usageStats);

                Vector2[] touchPointsArray = Utils.Instance.touchPoints.ToArray();
                int touchCounts = touchPointsArray.Length;

                GameData gameData = new GameData();
                gameData.appUsageStats = statsList[statsList.Count - 1];

                gameData.touchCounts = touchCounts;
                gameData.accelerometerCount = Utils.Instance.accelerometerCount;
                gameData.userId = deviceId;

                string payload = JsonConvert.SerializeObject(gameData);
                string payloadHash = Utils.Instance.ComputeSha256Hash(payload);


                RequestPlayIntegrity(payload, payloadHash);


            }
            catch (Exception e)
            {
                Debug.LogError("catch " + e);
                responseLabel.text = e + "";
                ResetTimePlayed();
            }


        }

        async void RequestPlayIntegrity(string payload, string payloadHash)
        {

            var ctx = BeamContext.Default;
            await ctx.OnReady;
            try
            {
                bool shouldVerify = await ctx.Microservices().GameServer().ShouldVerify();

                if (shouldVerify)
                {
                    StartPlayIntegrity(payloadHash, playIntegrityRes =>
                    {

                        if (playIntegrityRes.success == false)
                        {
                            Debug.LogError(playIntegrityRes.message);
                            responseLabel.text = playIntegrityRes.message;
                            ResetTimePlayed();
                        }
                        else
                        {
                            SendToGameServer(playIntegrityRes.token, payload);
                        }
                    });
                }
                else
                {
                    SendToGameServer(null, payload);
                }

            }
            catch (Exception e)
            {

                Debug.LogError(e.ToString());
                responseLabel.text = e.ToString();
            }


        }

        void ResetStatsLabels()
        {
            satsLabel.text = "0";
            withdrawPanelSats.text = "0";
            rewardsSliderInfo.text = "";
            satsConvertedLabel.text = "";
        }
        void SetStats()
        {

            satsLabel.text = currentStats.currentSats + "";
            satsConvertedLabel.text = "₿" + ((float)currentStats.currentSats / 100000000f).ToString("0.############");
            withdrawPanelSats.text = currentStats.currentSats + "";

            float currentEarningPercent = (float)currentStats.currentTimePlayed / (float)currentStats.currentRequiredTime;
            if (currentEarningPercent > 1)
            {
                currentEarningPercent = 1;
            }
            rewardsSlider.value = currentEarningPercent;
            long timePlayedCorrected = currentStats.currentTimePlayed;
            if (timePlayedCorrected > currentStats.currentRequiredTime)
            {
                timePlayedCorrected = currentStats.currentRequiredTime;
            }
            rewardsSliderInfo.text = FormatRewardsSliderInfo(timePlayedCorrected, currentStats.currentRequiredTime);

            if (currentStats.error)
            {
                responseLabel.text = currentStats.message;
            }
            else
            {
                responseLabel.text = currentStats.currentTimePlayed + "/" + currentStats.currentRequiredTime + " mins\n" + currentStats.currentSats + " sats";

            }
        }
        async void GetStats()
        {

            var ctx = BeamContext.Default;
            await ctx.OnReady;
            var result = await ctx.Microservices().GameServer().GetStats();
            currentStats = JsonConvert.DeserializeObject<RewardsResponse>(result);
            SetStats();
        }

        public void DownloadZBD()
        {
            Application.OpenURL("https://zbd.gg");
        }

        public void WithdrawBitcoin()
        {

            string username = usernameField.text;

            if (username.Length == 0)
            {
                ShowError("please enter your ZBD username");
                return;
            }
            withdrawButton.SetActive(false);


            try
            {
                StartPlayIntegrity("", playIntegrityRes =>
                {

                    if (playIntegrityRes.success == false)
                    {
                        Debug.LogError(playIntegrityRes.message);
                        responseLabel.text = playIntegrityRes.message;
                        ResetTimePlayed();
                    }
                    else
                    {

                        ContinueWithdraw(username, playIntegrityRes.token);
                    }

                });
            }
            catch (Exception e)
            {
                withdrawButton.SetActive(true);
                ShowError("error");
                Debug.LogError(e);

            }

        }

        async void ContinueWithdraw(string username, string token)
        {

            var ctx = BeamContext.Default;
            await ctx.OnReady;
            var result = await ctx.Microservices().GameServer().WithdrawBitcoin(username, token);

            SendToUsernameResponse res = JsonConvert.DeserializeObject<SendToUsernameResponse>(result);

            if (!res.success)
            {
                ShowError(res.message);
            }
            else
            {
                PlayerPrefs.SetString("username", username);
                ShowSuccess();
                satsLabel.text = "0";
                withdrawPanelSats.text = "0";
                rewardsSliderInfo.text = FormatRewardsSliderInfo(0, res.currentRequiredTime);
            }
            withdrawButton.SetActive(true);
        }

        string FormatRewardsSliderInfo(long currentTime, long requiredTime)
        {
            return currentTime + "/" + requiredTime + " mins approx";
        }
        async void SendToGameServer(string token, string payload)
        {
            var ctx = BeamContext.Default;
            await ctx.OnReady;
            try
            {
                var result = await ctx.Microservices().GameServer().SendPlaytime(token, payload);

                currentStats = JsonConvert.DeserializeObject<RewardsResponse>(result);
                Debug.Log("res " + result);

                SetStats();

                ResetTimePlayed();

            }
            catch (Exception e)
            {

                Debug.LogError(e);
                responseLabel.text = e.ToString();

            }
        }

        void StartPlayIntegrity(string payloadHash, Action<PlayIntegrityResponse> callback)
        {
            try
            {

                StartCoroutine(PrepareIntegrityTokenCoroutine(callback, payloadHash));

            }
            catch (Exception e)
            {
                PlayIntegrityResponse res = new PlayIntegrityResponse();
                res.success = false;
                res.message = "err " + e.ToString();
                callback(res);
            }
        }




        IEnumerator PrepareIntegrityTokenCoroutine(Action<PlayIntegrityResponse> callback, string payloadHash)
        {

            PlayIntegrityResponse res = new PlayIntegrityResponse();
            var standardIntegrityManager = new StandardIntegrityManager();

            var integrityTokenProviderOperation =
              standardIntegrityManager.PrepareIntegrityToken(
                new PrepareIntegrityTokenRequest(cloudProjectNumber));

            yield return integrityTokenProviderOperation;

            if (integrityTokenProviderOperation.Error != StandardIntegrityErrorCode.NoError)
            {

                res.success = false;
                res.message = "StandardIntegrityAsyncOperation failed with error: " +
                              integrityTokenProviderOperation.Error;

                Debug.LogError(res.message);
                callback(res);
                yield return null;


            }
            else
            {
                var integrityTokenProvider = integrityTokenProviderOperation.GetResult();

                String requestHash = payloadHash;
                var integrityTokenOperation = integrityTokenProvider.Request(
                  new StandardIntegrityTokenRequest(requestHash)
                );


                yield return integrityTokenOperation;

                if (integrityTokenOperation.Error != StandardIntegrityErrorCode.NoError)
                {
                    res.message = "StandardIntegrityAsyncOperation failed with error: " +
                            integrityTokenOperation.Error;
                    Debug.LogError(res.message);
                    callback(res);
                    yield return null;

                }
                else
                {
                    var integrityToken = integrityTokenOperation.GetResult();
                    res.success = true;
                    res.token = integrityToken.Token;
                    callback(res);

                }
            }

        }


        IEnumerator CountdownRoutine()
        {
            while (currentTime >= 0)
            {
                yield return new WaitForSeconds(1f);


                currentTime--;

                if (currentTime == 0)
                {
                    CheckTimePlayed();
                }
                else if (currentTime > 0)
                {
                    countdownLabel.text = currentTime.ToString("0");
                }
            }
        }

    }
}
