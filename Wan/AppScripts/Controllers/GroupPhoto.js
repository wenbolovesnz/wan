wan.controller('groupPhotoCtrl',
    ['$scope', 'datacontext', 'userService', '$location',
    function ($scope, datacontext, userService, $location) {

        $scope.uploadingOn = false;
        $scope.errorMsg = null;

        $scope.removePhotoFromGroup = function(photo) {
            var toRemove = _.find($scope.group.groupPhotos, function(p) {
                return p.id == photo.id;
            });

            removeItemFromArray($scope.group.groupPhotos, toRemove);

            $scope.group.$save();

        }


        function removeItemFromArray(array, item) {
            var index = _.indexOf(array, item);
            if (index != -1) {
                array.splice(index, 1);
            }
        };

        $scope.setAsBackgroupImage = function (photo) {
            $scope.group.backgroundImage = photo.url;
            $scope.group.$save();
        }

        $scope.uploadGroupPhoto = function () {
            $scope.uploadingOn = true;


            $.ajaxFileUpload(
                {
                    url: "Account/UploadGroupPhoto",
                    secureuri: false,
                    fileElementId: "uploadInputElement",
                    dataType: "html",
                    data: {
                        groupId : $scope.group.id
                    },
                    success: function (data, status) {
                        if (data.succeeded) {
                            $scope.group.groupPhotos.push(
                            {
                                id: data.groupPhoto.Id,
                                url: data.groupPhoto.Url,
                                groupId: data.groupPhoto.GroupId
                            });
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