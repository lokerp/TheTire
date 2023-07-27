mergeInto(LibraryManager.library, {

    SaveData: function (jsonDataUTF8) {
        let jsonData = UTF8ToString(jsonDataUTF8);
        SaveData(jsonData);
    },

    LoadData: function () {
        LoadData();
    },

    ShowFullscreenAdv: function () {
        ShowFullscreenAdv();
    },

    ShowRewardedAdv: function () {
        ShowRewardedAdv();
    },

    HasRated: function () {
        HasRated();
    },

    SetLeaderboardScore: function (distance) {
        SetLeaderboardScore(distance);
    },

    GetLeaderboard: function () {
        GetLeaderboard();
    },

    GetLeaderboardPlayerEntry: function () {
        GetLeaderboardPlayerEntry();
    },

    IsPlayerAuth: function () {
        return IsPlayerAuth();
    },

    HasPlayerPermission: function () {
        return HasPlayerPermission();
    },

    RequestPlayerPermission: function () {
        RequestPlayerPermission();
    },

    GetPlayerInfo: function () {
        let jsonPlayerInfo = GetPlayerInfo();
        var bufferSize = lengthBytesUTF8(jsonPlayerInfo) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(jsonPlayerInfo, buffer, bufferSize);
        return buffer;
    },

    Auth: async function () {
        await Auth();
    },

    OnGameReady: function () {
        OnGameReady();
    }

});

//mergeInto(LibraryManager.library, {

//    SaveData: function (jsonData) {
//        if (!isPlayerAuth) {
//            return null;
//        }
//        data = JSON.parse(UTF8ToString(jsonData));
//        player.setData(data);
//    },

//    LoadData: function () {
//        console.log("Loading data from server. IsPlayerAuth:", isPlayerAuth);
//        var loadStatusStr;
//        var resultDataStr;

//        if (!isPlayerAuth) {
//            resultDataStr = "";
//            loadStatusStr = "NOTAUTH"
//            gameInstance.SendMessage('APIBridge', 'LoadingStatusCallback', loadStatusStr);
//            gameInstance.SendMessage('APIBridge', 'LoadedDataCallback', resultDataStr);
//        }

//        player.getData()
//            .then((obj) => {
//                resultDataStr = JSON.stringify(obj);
//                loadStatusStr = "SUCCESS";
//            })
//            .catch(() => {
//                resultDataStr = "";
//                loadStatusStr = "ERROR";
//            })
//            .finally(() => {
//                gameInstance.SendMessage('APIBridge', 'LoadingStatusCallback', loadStatusStr);
//                gameInstance.SendMessage('APIBridge', 'LoadedDataCallback', resultDataStr);
//            });
//    },

//    ShowFullscreenAdv: function () {
//        ysdk.adv.showFullscreenAdv();
//    },

//    ShowRewardedAdv: function () {
//        ysdk.adv.showRewardedVideo({
//            callbacks: {
//                onOpen: () => {
//                    gameInstance.SendMessage('APIBridge', 'RaiseOnAdvertisementOpenEvent');
//                },
//                onRewarded: () => {
//                    gameInstance.SendMessage('APIBridge', 'RaiseOnAdvertisementCloseEvent', true);
//                },
//                onClose: () => {
//                    gameInstance.SendMessage('APIBridge', 'RaiseOnAdvertisementCloseEvent', false);
//                },
//                onError: (e) => {
//                    gameInstance.SendMessage('APIBridge', 'RaiseOnAdvertisementCloseEvent', false);
//                }
//            }
//        });
//    },

//    HasRated: function () {
//        ysdk.feedback.canReview()
//            .then(({ value, reason }) => {
//                if (!value && reason == "GAME_RATED")
//                    return true;
//                else
//                    return false;
//            });
//    },

//    SetLeaderbordScore: function (distance) {
//        if (isPlayerAuth) {
//            lb.SetLeaderbordScore('records', distance);
//        }
//    }

//});