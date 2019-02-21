// toastmessage
function popupMessage(data, then) {
	$().toastmessage('showToast', { text: data.msg, sticky: data.result != 'success', type: data.result });
	if (then && jQuery.isFunction(then[data.result]))
		then[data.result](data);
}

$(function () {

	// ajax modal
	$(document).on('click.bs.modal.data-api', '[data-toggle="ajax-modal"]', function (event) {
		
		var $this = $(this),
			url = $this.data('url'),
			$target = $($this.data('target'));
		
		$.get(url, function (response) {
			// ajax get form content
			$target
				.html(response)
				.modal('show')
				.find('.form-control').first().focus();
		});

	});


});


// summer-edit upload file

function sendFile(file, editor, welEditable) {
	if (file != null) {
		data = new FormData();
		data.append("file", file);
		$.ajax({
			data: data,
			type: "POST",
			url: "/Attachment/ImgFile",
			cache: false,
			contentType: false,
			processData: false,
			success: function (result) {
				editor.insertImage(welEditable, result.url, result.fileName);
			}
		});
	}
}
