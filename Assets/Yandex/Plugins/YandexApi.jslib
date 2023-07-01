mergeInto(LibraryManager.library, {

    SaveData: function (jsonData) {
        data = JSON.parse(jsonData);
        player.setData(data);
    },

    LoadData: function (hasEnded) {
        player.getData().then((obj) => {
            hasEnded = true;
            return JSON.stringify(obj);
        });
    },

    ShowFullscreenAdv: function () {
        ysdk.adv.showFullscreenAdv();
    },

    ShowRewardedAdv: function () {
        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    gameInstance.SendMessage('YandexAPI', 'RaiseOnAdvertisementOpenEvent');
                },
                onRewarded: () => {
                    gameInstance.SendMessage('YandexAPI', 'RaiseOnAdvertisementCloseEvent', true);
                },
                onClose: () => {
                    gameInstance.SendMessage('YandexAPI', 'RaiseOnAdvertisementCloseEvent', false);
                },
                onError: (e) => {
                    gameInstance.SendMessage('YandexAPI', 'RaiseOnAdvertisementCloseEvent', false);
                }
            }
        })
    }

});