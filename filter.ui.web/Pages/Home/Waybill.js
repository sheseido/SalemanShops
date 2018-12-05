angular.module('centerEndApp').controller('waybillCtrl', function ($scope, $http, $timeout, $ocLazyLoad, $element, NgTableParams, Upload, toaster) {
    $scope.pageBase = "Waybill";
    $scope.pageTitle = "运单";
    $scope.current = {};
    $scope.EditTitle = "";
    $scope.condition = {};

    $scope.sub = [];
    var subTabs = [];

    $scope.subTabs2 = subTabs;


    $scope.add = function () {
        $scope.current.PlantFormType = 1;
        $scope.subTabs = $scope.subTabs2;
        $scope.sub.detail = "/Pages/Home/" + $scope.pageBase + "Add.html?v=" + new Date().getTime();
        $scope.EditMode = "新建";
        $scope.current = {};
        $element.find(".maintain" + $scope.pageBase + "").modal({
            backdrop: false
        });
    };

    $scope.edit = function (item) {
        $scope.subTabs = $scope.subTabs2;
        $scope.sub.detail = "/Pages/Home/" + $scope.pageBase + "Edit.html?v=" + new Date().getTime();
        $scope.EditMode = "维护";
        $scope.doGet("/Shops/Info/" + item.Id, null, function (data) {
            $scope.current = data.Data;
        });

        $element.find(".maintain" + $scope.pageBase + "").modal({
            backdrop: false
        });
    };

    $scope.save = function (item) {
        $scope.doPost("/Shops/Save", item, function (data) {
            if (data.Result) {
                $scope.tableParams.reload();
                $element.find(".maintain" + $scope.pageBase).modal("hide");
            } else {
                toaster.warning(data.Message);
            }
        });
    };

    $scope.delete = function () {
        var items = [];
        for (var k in $scope.checkboxes.items) {
            if ($scope.checkboxes.items.hasOwnProperty(k)) {
                if ($scope.checkboxes.items[k]) {
                    items.push(parseInt(k));
                }
            }
        }

        if (items.length == 0) {
            toaster.warning("未选择删除项");
            return;
        }

        if (confirm("删除数据无法恢复，是否确认?")) {
            var join = items.join();

            $scope.doPost("/Shops/Delete?ids=" + join, null, function (response) {
                if (response.Result) {
                    $scope.tableParams.reload();
                } else {
                    toaster.warning(response.Message);
                }
            });
        }
    };

    $scope.checkboxes = { 'checked': false, items: {} };

    $scope.search = function () {
        $scope.tableParams.$params.page = 1;
        $scope.tableParams.reload();
    };

    $scope.reset = function () {
        $scope.condition = {};
        $scope.tableParams.$params.page = 1;
        $scope.tableParams.reload();
    };

    $scope.tableParams = new NgTableParams({
        page: 1,
        count: 10
    }, {
            total: 99,
            getData: function ($defer, params) {
                $scope.checkboxes = { 'checked': false, items: {} };
                $scope.doGet("/Waybill/GetPagedWaybills?page=" + params.page() + "&size=" + params.count(), { params: $scope.condition }
                    , function (data) {
                        $timeout(function () {
                            params.total(data.Data.TotalCount);
                            $defer.resolve(data.Data.Items);
                        }, 500);
                    });
            }
        });

    $scope.endExtend = function () {
        $scope.tableParams.reload();
    };

    /** 上传文件 */
    $scope.upload = function (file) {
        if (file) {
            Upload.upload({
                url: "/Waybill/Import",
                data: {
                    file: file,
                    request: $scope.current,
                }
            }).then(function (resp) {
                if (resp.data.Result) {
                    toaster.info("操作成功");
                    $scope.tableParams.reload();
                }
                else
                    toaster.warning(resp.data.Message);
            }, function (resp) {
                toaster.warning('上传失败 -> ' + resp.data.Result.message);
            }, function (evt) {
                //var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                //console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
            });
        }
    };

    $scope.export = function () {

        var key = "";
        if ($scope.condition.Key != undefined) {
            key = $scope.condition.Key;
        }

        var shopname = "";
        if ($scope.condition.ShopName != undefined) {
            shopname = $scope.condition.ShopName;
        }

        var saleman = "";
        if ($scope.condition.SalemanName != undefined) {
            saleman = $scope.condition.SalemanName;
        }

        var beginDate = "";
        if ($scope.condition.beginDate != undefined)
            beginDate = $scope.condition.beginDate.toISOString();

        var endDate = "";
        if ($scope.condition.endDate != undefined)
            endDate = $scope.condition.endDate.toISOString();
        
        document.location.href = "/Saleman/ExportSalemanAllWaybill?Key=" + key + "&ShopName=" + shopname + "&SalemanName=" + saleman + "&beginDate=" + beginDate + "&endDate=" + endDate;
        $scope.tableParams.reload();
    };
});