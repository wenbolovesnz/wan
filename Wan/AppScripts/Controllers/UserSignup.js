wan.controller('UserSignupCtrl',
    ['$scope', 'hub', 'userService', '$http', '$routeParams', '$location',
function ($scope, hub, userService, $http, $routeParams, $location) {

    $scope.emailRegister = "";

    $scope.passwordRegister = '';
    $scope.passwordRegister2 = '';

    $scope.differentPasswordError = false;

    $scope.register = function () {

        if ($scope.passwordRegister != $scope.passwordRegister2) {
            $scope.differentPasswordError = true;
        } else {
            var loginModel = {
                UserName: $scope.emailRegister,
                Password: $scope.passwordRegister,
                ConfirmPassword: $scope.passwordRegister2
            };

            $http.post('/Account/RegisterAjax', loginModel)
                .success(function (data, status, headers, config) {
                    if (data.status) {
                        userService.isLogged = true;
                        userService.username = data.userName;
                        $location.path("/");
                    }
                })
                .error(function (data, status, headers, config) {

                });
        }
    };

}]);