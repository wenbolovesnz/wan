
wan.controller('HomeCtrl',
    ['$scope', 'datacontext', 'hub',
    function ($scope, datacontext, hub) {

        $scope.title = "Home";

        $scope.groups = [];
        
        $scope.groups = datacontext.getAllGroups().query();

        hub.client.showNewGroup = function onShowNewGroup(group) {
            var newGroup = {
                groupName: group.GroupName,
                description: group.Description,
                createdDate: group.CreatedDate,
                users: group.Users
            };            
            $scope.groups.push(newGroup);
            $scope.$apply();
        };


        //$scope.refresh = function () {
        //    $scope.formDefinitionSets = datacontext.formDefinitionSets().query();
        //};

        //$scope.viewFormDefinitionVersion = function (formDefinitionModel) {
        //    window.location = '/#newForm/' + '?formDefinitionId=' + formDefinitionModel.id;
        //};


    }]);