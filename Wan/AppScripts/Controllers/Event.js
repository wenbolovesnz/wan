
wan.controller('EventCtrl',
    ['$scope', 'datacontext', '$routeParams','userService',
    function ($scope, datacontext, $routeParams, userService) {

        $scope.isCurrentUserManager = false;
        $scope.updating = false;
        $scope.isEdit = false;
        $scope.isAdmin = userService.isAdmin;

        $scope.uploadingOn = false;
        $scope.errorMsg = null;
        
        $scope.event = datacontext.event().get({id: $routeParams.eventId}, function(result) {

            $("#eventAd").attr("src", $scope.event.adUrl);


            $scope.isCurrentUserManager = _.find($scope.event.group.groupManagers, function (u) {
                                                    return u.id == userService.id;
            });
            
            $scope.isCurrentUserInGroup = _.find($scope.event.group.users, function (u) {
                return u.id == userService.id;
            });

            $scope.canCreateMessage = userService.isLogged && $scope.isCurrentUserInGroup;

            $scope.sponsors = datacontext.sponsor().query({ groupId: $scope.event.group.id }, function (dsfds) {
                datacontext.clientData.put('sponsors', $scope.sponsors);

                $scope.sponsorsSelect = [];
                _.each($scope.sponsors, function(s) {
                    var isInEvent = _.find($scope.event.sponsors, function(sp) {
                        return sp.id == s.id;
                    });

                    var selectItem = {
                        icon: s.photoUrl != null ? '<img src=' + s.photoUrl + '>' : '<img  src="/Content/Images/defaultgroup.png" />',
                        photoUrl : s.photoUrl,
                        id: s.id,
                        name: s.name,
                        ticked: isInEvent ? true : false

                };

                    $scope.sponsorsSelect.push(selectItem);
                });

            });

        });

        $scope.updateEvent = function () {
            $scope.updating = true;

            $scope.event.sponsors = _.filter($scope.sponsorsSelect, function(s) {
                return s.ticked == true;
            });

            $scope.event.$update(function() {
                $scope.updating = false;
                $scope.isEdit = false;
            });
        };

        $scope.createNewMessage = function () {
            $scope.updating = true;

            var newMessage = {
                id: 0,
                createdDate: new Date(),
                user: { userName: userService.username },
                message: $scope.message
            };

            $scope.event.eventMessages.push(newMessage);

            $scope.event.$update(function () {
                $scope.updating = false;
                $scope.message = "";
            });
        };

        $scope.showEdit = function() {
            $scope.isEdit = true;
        };

        $scope.cancelEdit = function() {
            $scope.isEdit = false;
        };

        $scope.uploadEventAd = function () {
            $scope.uploadingOn = true;

            $.ajaxFileUpload(
                {
                    url: "Account/UploadEventAd",
                    secureuri: false,
                    fileElementId: "uploadInputElement",
                    dataType: "html",
                    data: {
                        eventId: $scope.event.id,
                        adSiteUrl: $scope.event.adSiteUrl
                    },
                    success: function (data, status) {
                        if (data.succeeded) {
                            $scope.event.adUrl = data.url;
                            $("#eventAd").attr("src", $scope.event.adUrl);
                        } else {
                            $scope.errorMsg = "Error, your file type maybe not valid, please try again.";
                        }
                        $scope.uploadingOn = false;
                        $scope.$apply();
                    },
                    error: function (data, status, e) {
                        $scope.errorMsg = "Error, your file type maybe not valid, please try again.";
                        $scope.uploadingOn = false;
                        $scope.$apply();
                    }
                }
            );
        }


    }]);