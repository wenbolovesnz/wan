/* datacontext: data access and model management layer */

// create and add datacontext to the Ng injector
// constructor function relies on Ng injector
// to provide service dependencies
wan.factory('datacontext',
    ['Q', '$resource','$cacheFactory',
    function (Q, $resource, $cacheFactory) {

        var clientData = $cacheFactory('cachedData');

        clientData.put('groups', []);

        function getAllGroups() {
            return $resource('api/Group', {}, {
                query: { method: 'GET', isArray: true }
            });
        }
        
        function createGroup() {
            return $resource('api/Group/Create', {}, {
                create: { method: 'Post' }
            });
        }
        
        function updateGroup() {
            return $resource('api/Group/UpdateGroup', {}, {
                update: { method: 'Post' }
            });
        }
        
        
        
        return {
            getAllGroups: getAllGroups,
            createGroup: createGroup,
            clientData: clientData,
            updateGroup: updateGroup                        
        };

    }]);

wan.factory('userService', [function () {
    var sdo = {
        isLogged: false,
        username: '',
        id: 0,
        dob: null,
        city: "",
        nickName: "",
        createdDate: "",
        aboutMe:""
    };
    return sdo;
}]);


