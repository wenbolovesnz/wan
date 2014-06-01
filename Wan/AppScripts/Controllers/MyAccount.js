
wan.controller('MyAccountCtrl',
    ['$scope', 'datacontext', 'hub', 'userService', '$location', '$http',
    function ($scope, datacontext, hub, userService, $location, $http) {

        $scope.mygroups = true;
        $scope.changePassword = false;
        $scope.editMydetails = false;

        $scope.currentPassword = "";
        $scope.passwordRegister = "";
        $scope.passwordRegister2 = "";
        $scope.differentPasswordError = false;
        $scope.errorMessage = "";
        $scope.changeSucceeded = false;
        
        $scope.savePasswordChange = function () {
            if ($scope.passwordRegister != $scope.passwordRegister2) {
                $scope.differentPasswordError = true;
                $scope.errorMessage = "New password and Confirm password is different";
            } else {
                $scope.differentPasswordError = false;
                
                var model = {
                    OldPassword: $scope.currentPassword,
                    NewPassword: $scope.passwordRegister,
                    ConfirmPassword: $scope.passwordRegister2
                };
                $http.post('/Account/ChangePassword', model)
                    .success(function (data, status, headers, config) {
                        if (data.succeeded) {
                            $scope.differentPasswordError = false;
                            $scope.changeSucceeded = true;
                            setTimeout(function () {
                                $scope.changeSucceeded = false;
                                $scope.$apply();
                            }, 2500);
                        } else {
                            $scope.differentPasswordError = true;
                            $scope.changeSucceeded = false;
                            $scope.errorMessage = data.message;
                        }
                    })
                    .error(function (data, status, headers, config) {
                        $scope.differentPasswordError = true;
                        $scope.changeSucceeded = false;
                        $scope.errorMessage = "There was an error, please try again.";

                    });
            }

        };

        $scope.showMyGroups = function() {
            $scope.mygroups = true;
            $scope.changePassword = false;
            $scope.editMydetails = false;
        };
        
        $scope.showChangePassword = function () {
            $scope.mygroups = false;
            $scope.changePassword = true;
            $scope.editMydetails = false;
        };
        
        $scope.showEditMyDetails = function () {
            $scope.mygroups = false;
            $scope.changePassword = false;
            $scope.editMydetails = true;
        };
        
        
                
        $scope.groups = _.filter(datacontext.clientData.get('groups'), function (g) {
            return _.find(g.users, function(u) {
                return u.userName = userService.username;
            });
        });

    }]);