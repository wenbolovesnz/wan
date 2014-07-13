
wan.controller('NotificationMessagesCtrl',
    ['$scope', 'datacontext','$location','userService',
    function ($scope, datacontext, $location, userService) {


        $scope.processing = false;
        $scope.messages = [];
        
        datacontext.joinGroupRequest().query(function (result) {
            userService.messages = result;
            $scope.messages = userService.messages;
        });

        $scope.accept = function(message) {
            $scope.processing = true;
            message.isProcessed = true;
            message.isApproved = true;
            
            message.$update(function() {
                $scope.processing = false;
                removeItemFromArray(userService.messages, message);
            });

        };
        
        $scope.decline = function (message) {

            bootbox.prompt("Please state the reason for this?", function (result) {
                if (result === null) {

                } else {
                    $scope.processing = true;
                    message.isProcessed = true;
                    message.isApproved = false;
                    message.declineReason = result;

                    message.$update(function () {
                        $scope.processing = false;
                        removeItemFromArray(userService.messages, message);
                    });
                }
            });


        };
        
        function removeItemFromArray(array, item) {
            var index = _.indexOf(array, item);
            if (index != -1) {
                array.splice(index, 1);
            }
        };
    }]);