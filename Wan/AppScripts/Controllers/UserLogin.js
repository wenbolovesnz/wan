
wan.controller('UserLoginCtrl',
    ['$scope', 'hub', 'userService', '$http', '$routeParams',
function ($scope, hub, userService, $http, $routeParams) {

        $scope.emailAddress = "";

        $scope.password = '';

        $scope.login = function () {
            var loginModel = {
                UserName: $scope.emailAddress,
                Password: $scope.password
            };
            
            $http.post('/Account/LoginAjax', loginModel)
                .success(function (data, status, headers, config) {
                    if (data.status) {
                        userService.isLogged = true;
                        userService.username = data.userName;
                        if ($routeParams.returnUrl) {
                            window.location = "#/" + $routeParams.returnUrl;
                        } else {
                            window.location = "";
                        }
                    }
                })
                .error(function (data, status, headers, config) {
                    
                });
        };

    }]);