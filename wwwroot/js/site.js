var VoteMapSite = function () {

    var _sendGetHttpRequest = async function (_url) {
        return await fetch(_url, {
            method: 'GET',
            cache: 'no-cache',
            headers: {
                'content-type': 'application/json; charset=utf-8',
            }
        }).then(resp => {
            return resp.json()
        }).then(respJson => {
            return respJson;
        }).catch(error => {
            console.error('Catch Error:', error.message);
            return {
                retCode: 1,
                retMessage: error.message
            };
        }).then(response => {
            return response;
        });
    }

    //#region _commafy [千分位]
    var _commafy = function (num) {
        if (num) num = num.toFixed(0);
        num = num + "";
        let re = /(-?\d+)(\d{3})/
        while (re.test(num)) {
            num = num.replace(re, "$1,$2")
        }
        return num;
    }
    //#endregion

    return {
        // 初始化 Facebook 自訂客戶 Modal
        sendGetHttpRequest: async function (_url) {
            return await _sendGetHttpRequest(_url);
        },
        // 千分位
        commafy: function (num) {
            return _commafy(num);
        },
    };
}();