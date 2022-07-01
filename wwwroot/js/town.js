var mytown = function () {
	var currentFeature; 			// Current Selected Area;
	var cityMeta = {};				// 鄉鎮市區 投票資料
	var party = {};					// 政黨資料
	var tblVoteMetrics = []; 		// 表格選票指標
	var chartVoteMetrics = []; 		// 圓餅圖數據
	var donutchart_background = {};	// 圓餅圖背景色

	//#region _init_map [初始化OpenLayer]
	var _init_map = function () {
		let appView = new ol.View({
			center: ol.proj.fromLonLat([121.56283764144203, 25.03678997348458]),
			zoom: 13
		});

		let attribution = new ol.control.Attribution({
			collapsible: false,
			collapsed: true
		});

		const city = new ol.layer.Vector({
			source: new ol.source.Vector({
				url: `${joAPI.GetTaiwanCityTopo}`,
				format: new ol.format.TopoJSON({
					featureProjection: appView.getProjection()
				})
			}),
			style: cityStyle,
			zIndex: 50
		});

		var map = new ol.Map({
			layers: [city],
			target: 'map',
			view: appView,
			controls: ol.control.defaults({ attribution: false }).extend([attribution])
		});

		map.on('singleclick', function (evt) {
			appView.setCenter(evt.coordinate);
			map.forEachFeatureAtPixel(evt.pixel, function (feature, layer) {
				currentFeature = feature;
				let p = feature.getProperties();
				tblVoteMetrics = [];
				chartVoteMetrics = [['candidate_name', 'vote']];
				donutchart_background = {};
				$('.menu_header').text(p.COUNTYNAME + p.TOWNNAME);
				if (p.COUNTYNAME) {
					let idx = 0;
					const cityKey = p.COUNTYNAME + p.TOWNNAME;
					if (cityMeta[cityKey] && cityMeta[cityKey].candidate) {
						for (key in cityMeta[cityKey].candidate) {
							tblVoteMetrics.push({ "name": cityMeta[cityKey].candidate[key].candidate_name, "vote": cityMeta[cityKey].candidate[key].vote_count });
							chartVoteMetrics.push([cityMeta[cityKey].candidate[key].candidate_name, cityMeta[cityKey].candidate[key].vote_count]);
							donutchart_background[idx] = {
								color: party[cityMeta[cityKey].candidate[key].party_no].party_bg_color
							};
							idx += 1;
						}
						currentFeature.setStyle(cityStyle);
					}
				}

				mytown.googleDonutChart();
				mytown.refreshVoteDatatables();

				if (!$(".slide-menu").hasClass('active')) {
					$('.menu-item[data-target="#Panel1"]').click();
				}

			});
		});

		let positionFeature = new ol.Feature();

		positionFeature.setStyle(new ol.style.Style({
			image: new ol.style.Circle({
				radius: 6,
				fill: new ol.style.Fill({
					color: '#3399CC'
				}),
				stroke: new ol.style.Stroke({
					color: '#fff',
					width: 2
				})
			})
		}));

		new ol.layer.Vector({
			map: map,
			source: new ol.source.Vector({
				features: [positionFeature]
			})
		});
	}
	//#endregion

	//#region _fetch_vote_metrics 抓取選票資料
	var _fetch_vote_metrics = function () {
		VoteMapSite.sendGetHttpRequest(`${joAPI.GetPartyInfo}`).then((response) => {
			if (response.retCode == 0) {
				updateParty(response.result.list);
				VoteMapSite.sendGetHttpRequest(`${joAPI.GetTownMapMetric}?year=${$('#year').val()}&type=${$('#type').val()}`).then((response) => {
					if (response.retCode == 0) {
						updateTownMetrics(response.result.list);
					} else {
						console.error(response.retMessage);
					}
				});
			} else {
				console.error(response.retMessage);
			}
		});
	}
	//#endregion

	//#region _initVoteDatatables [初始化表格選票指標]
	var _initVoteDatatables = function () {
		$(`#tblVoteList`).DataTable({
			"scrollX": true,
			"paging": false,
			"ordering": true,
			"order": [[1, 'desc']],
			"info": false,
			"searching": false,
			"processing": false,
			"serverSide": false,
			"data": [],
			"columnDefs": [
				{ targets: 1, className: 'dt-body-right' },
			],
			"columns": [
				{ "data": 'name' },
				{ "data": 'vote', "render": function (data, type, row) { return `${VoteMapSite.commafy(data)}`; } },
			],
			footerCallback: function (row, data, start, end, display) {
				var api = this.api();

				// Remove the formatting to get integer data for summation
				var intVal = function (i) {
					return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
				};

				total_vote = api.column(1).data().reduce(function (a, b) { return intVal(a) + intVal(b); }, 0);
				// Update footer
				$(api.column(1).footer()).html(`${VoteMapSite.commafy(total_vote)}`);

			}
		}).draw();
	}
	//#endregion

	//#region _refreshVoteDatatables [刷新表格選票指標]
	var _refreshVoteDatatables = function () {
		$('#tblVoteList').DataTable().clear().draw();
		if (tblVoteMetrics && tblVoteMetrics.length > 0) {
			$('#tblVoteList').DataTable().rows.add(tblVoteMetrics).draw();
		}
	}
	//#endregion

	//#region _googleDonutChart [Google Donut Chart Draw]
	var _googleDonutChart = function () {
		const chartId = 'donutchart';
		if (chartVoteMetrics && chartVoteMetrics.length > 1) {
			let data = google.visualization.arrayToDataTable(chartVoteMetrics);
			let options = {
				pieHole: 0.3,
				chartArea: { width: '100%' },
				legend: { position: 'top', alignment: 'center' },
				slices: donutchart_background
			};

			let chart = new google.visualization.PieChart(document.getElementById(chartId));
			chart.draw(data, options);
		} else {
			$(`#${chartId}`).empty();
		}
	}
	//#endregion

	//#region cityStyle [OpenLayer City Style]
	function cityStyle(f) {
		const p = f.getProperties();
		const cityKey = p.COUNTYNAME + p.TOWNNAME;

		let color = '#ffffff';		// 預設 fill.color
		let textColor = '#000000';	// 預設 text.color
		let strokeWidth = 2;		// 地圖邊界寬度
		let strokeColor = 'rgba(255,255,255,1)';	// 地圖邊界顏色
		// 選擇的地圖變更邊界寬度及顏色
		if (f === currentFeature) {
			strokeColor = '#ff005d';
			strokeWidth = 5;
		}

		let party_no = 'TBD';
		if (cityMeta[cityKey]) {
			let max_vote_count = 0;	// 用來判斷最高票的政黨
			if (cityMeta[cityKey].party) {
				for (key in cityMeta[cityKey].party) {
					if (cityMeta[cityKey].party[key].vote_count > max_vote_count) {
						max_vote_count = cityMeta[cityKey].party[key].vote_count;
						party_no = key;
					}
				}
			}
		}
		// 設定地圖背景色以最高票政黨為主
		if (party[party_no]) {
			color = party[party_no].party_bg_color;
			textColor = party[party_no].party_text_color;
		}
		var baseStyle = new ol.style.Style({
			stroke: new ol.style.Stroke({
				color: strokeColor,
				width: strokeWidth
			}),
			fill: new ol.style.Fill({
				color: color
			}),
			text: new ol.style.Text({
				font: 'bold 22px monospace',
				fill: new ol.style.Fill({
					color: textColor
				}),
				textAlign: 'left'
			})
		});

		if (cityMeta[cityKey]) {
			let content = '';
			if (cityMeta[cityKey].candidate) {
				for (key in cityMeta[cityKey].candidate) {
					content += `${cityMeta[cityKey].candidate[key].candidate_name} : ${cityMeta[cityKey].candidate[key].vote_count}\n`
				}
			}
			baseStyle.getText().setText(`${p.TOWNNAME}\n${content}`);
		} else {
			baseStyle.getText().setText(`${p.TOWNNAME}`);
		}

		return baseStyle;
	}
	//#endregion 

	//#region updateParty [政黨資料]
	function updateParty(input) {
		input.forEach(element => {
			if (!party[element.party_no]) {
				party[element.party_no] = element;
			}
		});
	}
	//#endregion

	//#region updateTownMetrics [地區選票資料]
	function updateTownMetrics(input) {
		for (key in cityMeta) {
			cityMeta[key] = {};
		}
		input.forEach(element => {
			var cityKey = element.city + element.town;
			// 更新 Key Name
			switch (element.city) {
				case '台南市':
					cityKey = '臺南市' + element.town;
					break;
				case '台北市':
					cityKey = '臺北市' + element.town;
					break;
				case '台中市':
					cityKey = '臺中市' + element.town;
					break;
				case '台東縣':
					if (element.town !== '台東市') {
						cityKey = '臺東縣' + element.town;
					} else {
						cityKey = '臺東縣臺東市';
					}
					break;
				case '屏東縣':
					if (element.town === '霧台鄉') {
						cityKey = element.city + '霧臺鄉';
					}
					break;
				case '雲林縣':
					if (element.town === '台西鄉') {
						cityKey = element.city + '臺西鄉';
					}
					break;
			}

			if (!cityMeta[cityKey]) {
				const candidate = {
					candidate_no: element.candidate_no,
					candidate_name: element.candidate_name,
					party_no: element.political_party,
					vote_count: parseInt(element.vote_count),
				}
				const party = {
					party_no: element.political_party,
					vote_count: parseInt(element.vote_count),
				}
				cityMeta[cityKey] = {
					candidate: {},
					party: {},
				};

				cityMeta[cityKey].candidate[element.candidate_no] = candidate;
				cityMeta[cityKey].party[element.political_party] = party;
			} else {
				if (!cityMeta[cityKey].candidate[element.candidate_no]) {
					const candidate = {
						candidate_no: element.candidate_no,
						candidate_name: element.candidate_name,
						party_no: element.political_party,
						vote_count: parseInt(element.vote_count),
					}
					cityMeta[cityKey].candidate[element.candidate_no] = candidate;
				} else {
					// 理論上不應該存在
					cityMeta[cityKey].candidate[element.candidate_no].vote_count += parseInt(element.vote_count);
				}
				if (!cityMeta[cityKey].party[element.political_party]) {
					const party = {
						party_no: element.political_party,
						vote_count: parseInt(element.vote_count),
					}
					cityMeta[cityKey].party[element.political_party] = party;
				} else {
					cityMeta[cityKey].party[element.political_party].vote_count += parseInt(element.vote_count);
				}
			}
		});
	}
	//#endregion

	return {
		// 初始化OpenLayer
		init_map: function () {
			return _init_map();
		},
		// 抓取選票資料
		fetch_vote_metrics: function () {
			_fetch_vote_metrics();
		},
		// 初始化表格選票指標
		initVoteDatatables: function () {
			_initVoteDatatables();
		},
		// 刷新表格選票指標
		refreshVoteDatatables: function () {
			_refreshVoteDatatables();
		},
		// Google Donut Chart Draw
		googleDonutChart: function () {
			_googleDonutChart();
		}
	};
}();