/* datacontext: data access and model management layer */

// create and add datacontext to the Ng injector
// constructor function relies on Ng injector
// to provide service dependencies
wan.factory('datacontext',
    ['Q', '$resource',
    function (Q, $resource) {

        function getAllGroups() {
            return $resource('api/Group', {}, {
                query: { method: 'GET', isArray: true }
            });
        }
        
        return {
            getAllGroups: getAllGroups
        };

    }]);