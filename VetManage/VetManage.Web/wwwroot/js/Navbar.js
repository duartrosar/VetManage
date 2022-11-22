$(document).ready(function () {
	$.ajax({
		url: '/Account/GetUserAsync',
		type: 'POST',
		dataType: 'json',
		success: function (user) {
			$('#loggedUserImage').css("background-image", `url(${user.imageFullPath})`); 
			$('#loggedUserName').text(`${user.fullName}`);
		},
		error: function (ex) {
			alert('Failed to retrieve user.' + ex);
		}
	});
});

