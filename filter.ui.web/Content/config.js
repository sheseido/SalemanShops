/**
 * Created by Jeremy on 2017/6/14.
 */
var key_Token = "eock";
var key_Auth = "eoah";
function initHttp(app) {
    if(app) {
        app.factory('sessionInjector', function ($cookies) {
                return {
                    'request': function (request) {
                        if(request) {
                            var token = $cookies.get(key_Token);
                            if (token) {
                                token = decodeURI(token);
                                request.headers[key_Auth] = token;
                            }
                        }
                        return request;
                    },
                    'response': function (response) {
                        var auth = response.headers(key_Auth);
                        if(auth) {
                            //if (auth == "clear") {
                            //    $cookies.remove(key_Token);
                            //} else
                            {
                                auth = encodeURI(auth);
                                var date = new Date();
                                date.setDate(date.getDate() + 2);
                                $cookies.put(key_Token, auth, { path: "/", expires: date });
                            }
                        }
                        return response;
                    },
                    'responseError':function (error) {
                        if (error.status == 401) {
                            $cookies.remove(key_Token);
                        }
                        return error;
                    }
                };
            });
        app.config(["$httpProvider", function ($httpProvider) {
            $httpProvider.interceptors.push("sessionInjector");
        }]);
    }
}