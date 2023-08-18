mergeInto(LibraryManager.library, {

    SaveData: function (encryptedDataUTF8) {
        let encryptedData = UTF8ToString(encryptedDataUTF8);
        SaveData(encryptedData);
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