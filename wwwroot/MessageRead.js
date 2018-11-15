'use strict';

(function () {

    // The initialize function must be run each time a new page is loaded
    Office.initialize = function (reason) {
        $(document).ready(function () {
            getCallbackToken();
        });
    };

    function getItemRestId() {
        if (Office.context.mailbox.diagnostics.hostName === 'OutlookIOS') {
            // itemId is already REST-formatted
            return Office.context.mailbox.item.itemId;
        } else {
            // Convert to an item ID for API v2.0
            return Office.context.mailbox.convertToRestId(Office.context.mailbox.item.itemId, Office.MailboxEnums.RestVersion.v2_0);
        }
    }

    function getCallbackToken() {
        var options = {
            isRest: true 
        };
        Office.context.mailbox.getCallbackTokenAsync(options, cb);
    }

    function cb(asyncResult) {
        var accessToken = asyncResult.value;
        getHeaders(accessToken);
    }

    function displayHeaders(headers, accessToken, itemId) {
        var tbody = $('.prop-table');
        var item = Office.context.mailbox.item;

        tbody.append(makeTableRow("Email", item.from.emailAddress));
        tbody.append(makeTableRow("Name", item.from.displayName));
        tbody.append(makeTableRow("Created", item.dateTimeCreated));
        tbody.append(makeTableRow("Modified", item.dateTimeModified));

        var dregex = /Deferred-Delivery: [SMTWF]{1}[a-z]{2}, [1-9]{1,2} [JFMASOND]{1}[a-z]{2} [0-9]{4} \d{2}:\d{2}:\d{2}/g;
        var ipregex = /X-Originating-IP: \[([0123456789abcdef:\.]{7,40})\]/;

        var deferred = headers.match(dregex);
        var ip = headers.match(ipregex);

        if (deferred != null) {
            tbody.append(makeTableRow("Deferred", deferred));
        }
        tbody.append(makeTableRow("IP", ip[1]));
        
        $.ajax({
            type: "POST",
            url: "api/ip/",
            data: JSON.stringify({"ip":ip[1]}),
            dataType: 'json',
            headers: { 'Content-Type': 'application/json' }
        }).done(function (item) {
            var ipg = "";
            if (item.city != null) { ipg += item.city + " "; } //else { tbody.append(makeTableRow("city", ipg)); }
            if (item.state != null) { ipg += item.state + " "; }
            if (item.country != null) {
                ipg += item.country + ' <img src="Images/' + item.country + '.png" width="20" height="20">';
            }
            tbody.append(makeTableRow("Geolocation", ipg));
            if (item.asn != null) {
                tbody.append(makeTableRow("IP ASN", item.asn));
            }
        })
    }

    function getHeaders(accessToken) {

        var itemId = getItemRestId();
        var getMessageUrl = Office.context.mailbox.restUrl + '/v2.0/me/messages/' + itemId + "?$select=SingleValueExtendedProperties&$expand=SingleValueExtendedProperties($filter=PropertyId eq 'String 0x007D')";

        $.ajax({
            url: getMessageUrl,
            dataType: 'json',
            headers: { 'Authorization': 'Bearer' + accessToken, 'Accept': 'application/json; odata.metadata=none' }
        }).done(function (item) {
            displayHeaders(item.SingleValueExtendedProperties[0].Value, accessToken, itemId);
        })
    }

    function makeTableRow(name, value) {
        return $("<tr><td><strong>" + name +
            "</strong></td><td class=\"prop-val\"><code>" +
            value + "</code></td></tr>");
    }

})();
