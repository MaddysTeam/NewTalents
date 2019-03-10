// ajax
$.ajaxSetup({ cache: false });
var ajaxErrorMessage = "操作出现异常！";

// toastmessage
function popupMessage(data, then) {
	$().toastmessage('showToast', { text: data.msg, sticky: data.result != 'success', type: data.result });
	if (then && jQuery.isFunction(then[data.result]))
		then[data.result](data);
}

// ajax-btn
function beforeSend(theButton) {
	theButton.prop('disabled', true);
	theButton.find('i').removeClass().addClass('fa fa-spinner fa-spin');
	var span = theButton.find('span');
	span.attr('oldtext', span.text()).text('处理中...');
	return true;
}
function afterAction(theButton) {
	theButton.prop('disabled', false);
	theButton.find('i').removeClass().addClass('fa fa-floppy-o');
	var span = theButton.find('span');
	span.text(span.attr('oldtext'));
	return true;
}

// boot-grid
var gridOptions = {

	ajax: true,

	padding: 4,

	rowCount: [10, 20, 50/*, -1*/],

	labels: {
		all: "全部",
		infos: "显示 {{ctx.start}} 到 {{ctx.end}} 共 {{ctx.total}} 记录",
		loading: "数据加载中...",
		noResults: "无可寻记录",
		refresh: "重新加载",
		search: "搜索"
	},

	css: {
		iconRefresh: "glyphicon-search",
	},

	templates: {
		footer: "<div id=\"{{ctx.id}}\" class=\"{{css.footer}}\"><div class=\"row infoBar\"><p class=\"{{css.pagination}}\"></p><p class=\"{{css.infos}}\"></p></div></div>"
	},

	formatters: {

		"Boolean": function (column, row) { return row[column.id] ? "<i class=\"fa fa-check-square-o fa-lg\"></i>" : ""; },

		"Current": function (column, row) { return "$" + row[column.id]; },

		"DateTime": function (column, row) { return row[column.id] ? moment(row[column.id]).format('YYYY-MM-DD hh:mm:ss') : ''; },

		"DateOnly": function (column, row) { return row[column.id] ? moment(row[column.id]).format('YYYY-MM-DD') : ''; },

		"Email": function (column, row) { return "<a href='mailto:" + row[column.id] + "'>" + row[column.id] + "</a>"; },

		"Percent": function (column, row) { return row[column.id] + " %"; },

		"TimeOnly": function (column, row) { return row[column.id]; },

		"Url": function (column, row) {
			var text = row[column.id], url = text;
			if (!(typeof text === "string")) { url = text.url; text = text.text; }
			return "<a href='" + url + "' target='_blank'>" + text + "</a>";
		},

	},

	responseHandler: function (response) {
		if (response.errMessage) {
			alert(response.errMessage);
			return { current: 1, rowCount: 10, rows: [], total: 0 };
		}
		else {
			return response;
		}
	},

	loadDataErrorHandler: function (errMessage) {
		alert(errMessage);
	}

};

$(function () {

// district code
$('.dc-ajax').select2({
	ajax: {
		url: '/DistrictCode/Filter',
		dataType: 'json',
		type: 'post',
		delay: 250,
		data: function (params) {
			return {
				match: params.term, // search term
				page: params.page
			};
		},
		processResults: function (data, params) {
			params.page = params.page || 1;
			return {
				results: data.items,
				pagination: {
					more: (params.page * 30) < data.total
				}
			};
		},
		cache: false
	},
	escapeMarkup: function (markup) { return markup; },
	minimumInputLength: 2,
	templateResult: function (repo) {
		if (repo.loading) return repo.text;
		return "<adiv class='clearfix'>" + repo.name + "<span class='pull-right'>" + repo.id + "</span></div>";
	},
	templateSelection: function (repo) {
		return repo.name || repo.text;
	},
	inputTooShort: function (args) {
		var remainingChars = args.minimum - args.input.length;

		var message = 'Please ' + remainingChars + ' or more characters';

		return message;
	},

})

// student code
$('.std-ajax').select2({
	ajax: {
		url: '/StudentChoose/Filter',
		dataType: 'json',
		type: 'post',
		delay: 250,
		data: function (params) {
			var ret = {
				match: params.term, // search term
				page: params.page
			};
			if (getStdAjaxExt) { ret = getStdAjaxExt(ret); }
			return ret;
		},
		processResults: function (data, params) {
			params.page = params.page || 1;
			return {
				results: data.items,
				pagination: {
					more: (params.page * 30) < data.total
				}
			};
		},
		cache: false
	},
	escapeMarkup: function (markup) { return markup; },
	minimumInputLength: 1,
	templateResult: function (repo) {
		if (repo.loading) return repo.text;
		return "<adiv class='clearfix'>" + repo.name + "<span class='pull-right'>" + repo.roll + "</span></div>";
	},
	templateSelection: function (repo) {
		return repo.name || repo.text;
	},
	inputTooShort: function (args) {
		var remainingChars = args.minimum - args.input.length;

		var message = 'Please ' + remainingChars + ' or more characters';

		return message;
	},

})

})

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
				.modal({ backdrop: 'static', keyboard: false , show: true});
				//.find('.form-control').first().focus();
		});
	})

})

$(function () {

	$(".btn-expand").click(function (e) {
		e.preventDefault();

		var t = $(this).closest(".widget");

		if ($("body").hasClass("widget-expand") && $(t).hasClass("widget-expand")) {
			$("body, .widget").removeClass("widget-expand");
			$(".widget").removeAttr("style");
		} else {
			$("body").addClass("widget-expand");
			$(this).closest(".widget").addClass("widget-expand");
		}
		$(window).trigger("resize")
	})

})

function ajaxSubmitForm(selector) {
	$.validator.unobtrusive.parse(selector);

	selector.submit(function (e) {
		e.preventDefault();
		var $this = $(this);
		$this.valid() && $.post($this.attr('action'), $this.serialize(), function (data, status) {
			popupMessage(data, {
				success: function () {
					var afterSuccess = $this.data('afterSuccess');
					if (afterSuccess) {
						eval(afterSuccess);
					}
				}
			});
		})
	});
}

function ajaxBindFileUpload() {
	// dropzone
	Dropzone.autoDiscover = false;
	$('.dropzone').dropzone({
		addedContainer: '#flyArea',
		dictResponseError: '上传出错',
		dictFileTooBig: '上传文件大小({{filesize}}MiB) 最大文件大小 ({{maxFilesize}}MiB)',
		uploadMultiple: false,
		maxFilesize: 2046,
		init: function () {
			this.on("processing", function (i) {
				$('.progress').remove();
				$('#uploadName').parent().parent().append(function () {
					return '<div class="progress"><div class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"><span class="sr-only"></span></div></div>';
				})
			})
			this.on("totaluploadprogress", function (file, progress, bytesSent) {
				$(".progress-bar").css("width", parseInt(file) + "%");
			})
			this.on('success', function (file, data) {
				var url = $('#AttachmentUrl').val();
				url = url.length > 0 ? url + "|" : url;
				var name = $('#AttachmentName').val();
				name = name.length > 0 ? name + "|" : name;
				$('#AttachmentUrl').val(url + data.url);
				$('#AttachmentName').val(name + data.filename);
				var showName = data.filename.length > 40 ? data.filename.substring(0, 37) + "..." : data.filename;
				$('#uploadName').append('<p><label title="' + data.filename + '">' + showName + '</label> <button type="button" class="btn btn-danger btn-xs btn-delete" onclick="delAttachment(this)">删除</button></p>');
			});
			this.on('error', function (file, message) {
				popupMessage({ result: 'error', msg: message });
			});
		}
	});

	// proxy click event to dropzone.
	$('#btn-upload').on('click', function () {
		$('.dropzone').trigger('click');
		$(".progress").hide();
		$(".progress-bar").attr('style', 'width:0%');
	});
}

function ajaxSimpleFileUpload(dropzoneId, btnUploadId, whenSuccess, whenError) {
	// dropzone
	Dropzone.autoDiscover = false;
	var dropzoneId = '#' + dropzoneId;
	$(dropzoneId).dropzone({
		addedContainer: '#flyArea',
		dictResponseError: '上传出错',
		uploadMultiple: false,
		maxFilesize: 200,
		init: function () {
			this.on('success', function (file, data) {
				whenSuccess & whenSuccess(file,data);
			});
			this.on('error', function (file, message) {
				whenError & whenError(file,message);
			});
		}
	});

	// proxy click event to dropzone.
	$('#' + btnUploadId).on('click', function () {
		$(dropzoneId).trigger('click');
	});
}

//	删除附件

function delAttachment(e) {

	var current = $('.btn-delete').index(e);

	var name = $('#AttachmentName').val();
	var url = $('#AttachmentUrl').val();

	var nameArray = name.split('|');
	var tempName = '';
	$.each(nameArray, function (index, item) {
		if (current != index) {
			tempName += item + '|';
		}
	})

	var urlArray = url.split('|');
	var tempUrl = '';
	$.each(urlArray, function (index, item) {
		if (current != index) {
			tempUrl += item + '|';
		}
	})

	tempName = tempName.length > 0 ? tempName.substring(0, tempName.lastIndexOf('|')) : tempName;
	tempUrl = tempUrl.length > 0 ? tempUrl.substring(0, tempUrl.lastIndexOf('|')) : tempUrl;

	$('#AttachmentName').val(tempName);
	$('#AttachmentUrl').val(tempUrl);
	$('#uploadName p:eq(' + current + ')').remove();
	$(".progress").remove();
}

function insureSummernot(selector) {

	selector.summernote({
		height: 300,
		focus: true,
		placeholder: '请输入内容',
		toolbar: [
                //['fontname', ['fontname']], //字体系列                                 
                ['style', ['bold', 'italic', 'underline', 'clear']], // 字体粗体、字体斜体、字体下划线、字体格式清除       
               // ['font', ['strikethrough', 'superscript', 'subscript']], //字体划线、字体上标、字体下标   
                //['fontsize', ['fontsize']], //字体大小                                
                ['color', ['color']], //字体颜色     
                ['style', ['style']],//样式
                ['para', ['ul', 'ol', 'paragraph']], //无序列表、有序列表、段落对齐方式
                ['height', ['height']], //行高
                ['undo',['undo']], //撤销
                ['redo',['redo']], //取消撤销
		],
		callbacks: {
			onInit: function () {
				$node = $(this);
				$node.summernote('code', $('#' + $node.data('whatever')).val());
			},

			onBlur: function () {
				$node = $(this);
				$('#' + $node.data('whatever')).val($node.summernote('code'))
			},

			onImageUpload: function (files, editor, welEditable) {
				$node = $(this);
				summernoteSendFile(files[0], $node);
			},

			onImageUploadError: function () {
				popupMessage({ result: 'error', msg: '图片上传失败!<br/>保存当前信息，刷新重新来过' });
			}
		}
	});

}

// summer-edit upload file

function summernoteSendFile(file, node) {
	data = new FormData();
	data.append("file", file);

	var fileData = URL.createObjectURL(file);
	node.summernote('insertImage', fileData, function ($image) {
		$.ajax({
			url: "/Attachment/UploadFile",
			data: data,
			cache: false,	
			contentType: false,
			processData: false,
			dataType: "json",
			type: 'POST',
			success: function (result) {
				$image.attr('src', result.url);
			}
		});

	});
}

//	chat pie
function PieChart($select, data, options) {

	var defaultOptions = {
		series: {
			pie: {
				show: true,
				innerRadius: 0.4,
				label: {
					show: true,
					radius: 3 / 4,
					formatter: function (label, series) { return "<div class=\"donut-label\">" + label + "<br/>" + Math.round(series.percent) + "%</div>"; }
				}
			},
		},
		legend: { labelBoxBorderColor: "#ddd", backgroundColor: "none" },
		grid: {
			hoverable: true
		},
		colors: ["#d9d9d9", "#5399D6", "#d7ea2b", "#348fe2", "#49b6d6", "#f59c1a", "#727cb6", "#ff5b57"],
	};

	options = $.extend({}, defaultOptions, options);

	$.plot($select, data, options);

}

//	chart bar
function BarChart($select, data, options) {

	var defaultOptions = {
		series: {
			bars: {
				show: true,
				align: 'center',
				barWidth: 0.5,
				lineWidth: 0,
				label: {
					show: true
				},
				fillColor: {
					colors: [{
						opacity: 0.8
					}, {
						opacity: 0.8
					}]
				}
			}
		},
		legend: {
			show: true,
		},
		grid: { hoverable: true, borderWidth: 0, labelMargin: 0, axisMargin: 0, minBorderMargin: 0 },
		colors: ["#d9d9d9", "#5399D6", "#d7ea2b", "#348fe2", "#49b6d6", "#f59c1a", "#727cb6", "#ff5b57"]
	};

	options = $.extend({}, defaultOptions, options);

	$.plot($select, data, options);

	if (options.isUseTooltip) {

		InitialTooltip();

		$($select).UseTooltip();
	}
}

// chart	Tooltip
function InitialTooltip() {

	function showTooltip(x, y, color, contents) {
		$('<div id="tooltip">' + contents + '</div>').css({
			position: 'absolute',
			display: 'none',
			top: y - 40,
			left: x - 120,
			border: '2px solid ' + color,
			padding: '3px',
			'font-size': '9px',
			'border-radius': '5px',
			'background-color': '#fff',
			'font-family': 'Verdana, Arial, Helvetica, Tahoma, sans-serif',
			opacity: 0.9
		}).appendTo("body").fadeIn(200);
	}

	var previousPoint = null, previousLabel = null;

	$.fn.UseTooltip = function () {
		$(this).bind("plothover", function (event, pos, item) {
			if (item) {
				if ((previousLabel != item.series.label) || (previousPoint != item.dataIndex)) {
					previousPoint = item.dataIndex;
					previousLabel = item.series.label;
					$("#tooltip").remove();

					var x = item.datapoint[0];
					var y = item.datapoint[1];

					var color = item.series.color;

					showTooltip(item.pageX,
					item.pageY,
					color,
					"<strong>" + item.series.xaxis.ticks[x].label + " : " + y + "</strong>");
				}
			} else {
				$("#tooltip").remove();
				previousPoint = null;
			}
		});
	};
}

// 解决  editor 控件中的关闭选择图片，附件，video弹出窗时会关闭整个dialog 的bug

$('.close').click(function () {
	$(this).parents('.modal').eq(0).hide();
	return false;
});
$('.note-icon-link').parent().click(function () {
	$(this).parents('.note-editor').find('.modal').eq(0).show();
});
$('.note-icon-picture').parent().click(function () {
	$(this).parents('.note-editor').find('.modal').eq(1).show();
});
$('.note-icon-video').parent().click(function () {
	$(this).parents('.note-editor').find('.modal').eq(2).show();
});