  function ServerGroup(options){
    $.extend(this, {lastupdate: 0,servers : {}, panel:$('.servers-panel')},options);
    this.init = function(options){
      $.extend(this, options);
      var g=this;
      $.ajax( g.url )
      .done(function(o) {
        if($.isArray(o.servers))o.servers.forEach(function(v){
          var s = new Server(v);
          s.render(g.panel,v.id);
          g.servers[v.id] = s;
          if(v.projects)g.list.render(v);
        })
        g.RefreshStatus();
      })
    }
    this.RefreshStatus = function(){
      var c=this;
      $.each(c.servers, function(i,s){ s.addClass('server-comm') })
      $.ajax( "status/"+c.lastupdate )
      .done(function(o) {
        c.lastupdate = o.ch;
        $.each(o.s,function(i,s){
          if(c.servers[i])c.servers[i].update(s);
          if(s.projects)project_list.update(s.projects);
        })
        $.each(c.servers, function(i,s){ s.removeClass('server-comm-fail') })
      })
      .fail(function() {
        $.each(c.servers, function(i,s){ s.addClass('server-comm-fail') })
      })
      .always(function() {
        $.each(c.servers, function(i,s){ s.removeClass('server-comm') })
        setTimeout(function(){c.RefreshStatus()},2000)
      });
    }
  };




  function Server(options){
    this.options = options;
    this.renderValueControl = function(id,type){
      return { 
        id: type + '_' + id,
        $ctrl: $('<td id="'+(type + '_' + id)+'" class="server-'+type+'">'+
          '<div class="knob-slider"><input class="knob" data-width="60" data-height="60" data-angleOffset="180" data-fgColor="#6AA6D6" data-skin="tron" data-thickness=".2" value=""></div>'+
          '<div class="chart" style="display: inline-block; width: 85px; height: 70px;"></div>'+
          '</td>'),
        init: function(){
          DrawKnob(this.$ctrl.find('.knob').val(0));
          this.data = new Array(60)//$.map(new Array(60),function(v,i){return (i%2==0) ? 100 : 0});
          this.data.fill(0);
          this.series = [this.data];
          this.chart = new Chartist.Line('#'+this.id+' .chart', {
            labels:$.map(new Array(60),function(v,i){ return (i%10 == 0)?(''+i):null}),
            series: this.series
          }, {
            low: 0,
            axisX:{showLabel: 0,offset:0},
            axisY:{showLabel: 0,offset:0},
            showArea: true,
            showPoint: false,
            fullWidth: true
            ,high: 100,
            low: 0,
            step: 25
          });
        },
        val:function(v){
          RedrawKnob(this.$ctrl.find('.knob'),v);
          this.series[0] = this.series[0].slice(1);
          this.series[0].push(v);
          this.chart.update();
        }
      }
    }
    this.render = function($cont,id){
      this.id = id;
      this.$header = $('<div class="panel-heading"><h3 class="panel-title">'+this.options.name+' <span class="status-light connect"><i class="fa fa-plug"></i></span><span class="status-light warning"><i class="fa fa-warning"></i></span><span class="status-light server-up-time"><i class="fa fa-clock"></i><span class="label"></span></span></h3></div>')
      this.$values = {
        cpu: this.renderValueControl(this.id,'cpu'),
        memory: this.renderValueControl(this.id,'memory'),
        disk: this.renderValueControl(this.id,'disk')
      }
      this.$serverblock = $('<div class="panel panel-default">').append(
        this.$header,
        $('<table class="server-block">').append(
          $('<tr>').append(
            this.$values.cpu.$ctrl,
            this.$values.memory.$ctrl,
            this.$values.disk.$ctrl
          ),
          $('<tr class="labels"><td class="label-cpu">Cpu</td><td class="label-memory">Memory</td><td class="label-disk">Disk</td></tr>')
        )
      )
      $cont.append(this.$serverblock)
      this.$values.cpu.init();
      this.$values.memory.init();
      this.$values.disk.init();
      this.val=function(name,v){
        if(typeof v == 'undefined')return;
        this.$values[name].val(v)
      }
      this.addClass = function(cl){
        this.$serverblock.addClass(cl);
      }
      this.removeClass = function(cl){
        this.$serverblock.removeClass(cl);
      }
    }
    this.update = function(s){
      if(!s.perf)return;
      this.val('cpu',s.perf.c);
      //.cpu-warining .cpu-memory .cpu-disk
      this.val('memory',s.perf.m);
      this.val('disk',s.perf.d);
      this.$header.find(".server-up-time > span.label").text(moment.duration(s.perf.u * 1000).format("d[d] h:mm:ss"));
    }
  }

  function ProjectList(selector){
    this.$panel = $(selector)
    this.$table = $(selector).find('table');
    this.$header = $(selector).find('.panel-heading')
    
//      this.$header = $('<div class="panel-heading"><h3 class="panel-title">'+this.options.name+' <span class="status-light connect"><i class="fa fa-plug"></i></span><span class="status-light warning"><i class="fa fa-warning"></i></span><span class="status-light server-up-time"><i class="fa fa-clock"></i><span class="label"></span></span></h3></div>')
    this.render = function(srv){
      if(!srv.projects)return;
      this.$table.find('tbody').append(
        $.map(srv.projects, function(v,i){
          return $('<tr>').attr('id', 'p'+i).addClass('s'+srv.id).data('proj',{srv:srv, prjid:i, name:v.name}).append(
            '<td class="col col-row-state"><i class="fa fa-caret-right"></i></td>'+
            '<td class="col col-sever">'+srv.name+'</td>'+
            '<td class="col col-prj-name">'+ v.name +'</td>'+
            '<td class="col col-build-status"></td>'+
            '<td class="col col-prj-time"></td>'+
            '<td class="col col-prj-next"></td>'+
            '<td class="col col-cc-status"></td>'+
            '<td class="col col-activity"></td>'+
            '<td class="col col-buttons">'+
              '<button type="button" item-cmd="abort" class="btn btn-xs btn-primary cmd-btn cmd-abort">Abort</button>'+
              '<button type="button" item-cmd="force" class="btn btn-xs btn-warning cmd-btn cmd-force">Force</button>'+
              '<button type="button" item-cmd="start" class="btn btn-xs btn-success cmd-btn cmd-start">Start</button>'+
              '<button type="button" item-cmd="stop" class="btn btn-xs btn-danger cmd-btn cmd-stop">Stop</button>'+
            '</td>'
          )
        })
      )
      var h = this.$header.find('h3')
      var p = this.$panel;
      this.renderHdrVal(h,'total','plug',function(){p.removeClass('show-fail show-success show-running show-building')})
      this.renderHdrVal(h,'success','check-circle',function(){p.toggleClass('show-success')})
      this.renderHdrVal(h,'fail','times-circle',function(){p.toggleClass('show-fail')})
      this.renderHdrVal(h,'building','gears',function(){p.toggleClass('show-building')})
      this.renderHdrVal(h,'sleep','moon-o',function(){})
      this.renderHdrVal(h,'running','play',function(){p.toggleClass('show-running')})
      this.$table.tablesorter()
      this.updateTotals();
    }
    this.renderHdrVal = function($p,cl,ico, click){
      var c=this
      if(!$p.find('.'+cl).length){ 
        $p.append(
          $('<span class="status-light '+cl+' clickable"><i class="fa fa-'+ico+'"></i><span class="value"></span></span>')
          .click(function(){click(c.$table)}) 
        )
      }
    }
    this.updateTotals = function(){
      var totals = {
        total:this.$table.find('tbody tr').length,
        success:this.$table.find('tbody tr.bs-success').length,
        fail:this.$table.find('tbody tr.bs-fail').length,
        building:this.$table.find('tbody tr.caa-building').length,
        sleeping:this.$table.find('tbody tr.caa-sleep').length,
        running:this.$table.find('tbody tr.ccs-running').length
      }
      this.$header.find('.total .value').text(totals.total)
      this.$header.find('.success .value').text(totals.success)
      this.$header.find('.fail .value').text(totals.fail)
      this.$header.find('.building .value').text(totals.building)
      this.$header.find('.sleep .value').text(totals.sleeping)
      this.$header.find('.running .value').text(totals.running)
      document.title = 'T'+totals.total+' B'+this.$table.find('tbody tr.caa-building').length + ' S'+totals.success+' F'+totals.fail
    }
    this.showBuildStatus = function($tr,d){
        switch(d.bs){
          case 1:
            $tr.removeClass('bs-fail bs-unk').addClass('bs-success');
            $tr.find('.col-build-status').html('<i class="fa fa-check-circle"></i>Success'); 
            break;
          case 2:
            $tr.removeClass('bs-success bs-unk').addClass('bs-fail');
            $tr.find('.col-build-status').html('<i class="fa fa-times-circle"></i>Fail'); 
            break;
          default:
            $tr.removeClass('bs-fail bs-success').addClass('bs-unk');
            $tr.find('.col-build-status').html('<i class="fa fa-question-circle"></i>Unknown'); 
        }
    }
    this.showCCStatus = function($tr,d){
        switch(d.ccs){
          case 1:
            $tr.removeClass('ccs-stopping ccs-running ccs-unk').addClass('ccs-stopped');
            $tr.find('.col-cc-status').html('<i class="fa fa-stop"></i>Stopped'); 
            break;
          case 2:
            $tr.removeClass('ccs-stopped ccs-running ccs-unk').addClass('ccs-stopping');
            $tr.find('.col-cc-status').html('<i class="fa fa-step-forward"></i>Stopping'); 
            break;
          case 3:
            $tr.removeClass('ccs-stopped ccs-stopping ccs-unk').addClass('ccs-running');
            $tr.find('.col-cc-status').html('<i class="fa fa-play"></i>Running'); 
            break;
          default:
            $tr.removeClass('ccs-stopped ccs-stopping ccs-running').addClass('ccs-unk');
            $tr.find('.col-cc-status').html('<i class="fa fa-question-circle"></i>Unknown'); 
        }
    }
    this.showCCActivity = function($tr,d){
        switch(d.cca){
          case 1: //Sleeping
            $tr.removeClass('caa-building caa-check caa-pending').addClass('caa-sleep');
            $tr.find('.col-activity').html('<i class="fa fa-moon-o"></i>Sleep'); 
            break;

          case 2: //Building
            $tr.removeClass('caa-sleep caa-check caa-pending').addClass('caa-building');
            $tr.find('.col-activity').html('<i class="fa fa-gears"></i>Building'); 
            break;

          case 3: //CheckingModifications
            $tr.removeClass('caa-sleep caa-building caa-pending').addClass('caa-check');
            $tr.find('.col-activity').html('<i class="fa fa-search"></i>Check'); 
            break;

          case 4: //Pending
            $tr.removeClass('caa-sleep caa-building caa-check').addClass('caa-pending');
            $tr.find('.col-activity').html('<i class="fa fa-clock-o"></i>Pending'); 
            break;

            
          default: //Unknown
            //$tr.removeClass('cca-fail bs-success').addClass('bs-unk');
            $tr.find('.col-activity').html('<i class="fa fa-question-circle"></i>Unknown'); 

        }
    }
    this.updateProjectRow = function($tr,v){
      var c=this
      $tr.addClass('row-update')
      var d = $.extend($tr.data('proj'), v);
      d.nb = (d.srv.dtb + v.nb) * 1000;
      $tr.data('proj',d)
      $tr.find('.col-prj-time').text(d.abt)
      c.showBuildStatus($tr,d)
      c.showCCStatus($tr,d)
      c.showCCActivity($tr,d)
      $tr.find('.col-prj-next').text(moment.duration(moment() - moment(d.nb)).format())
      setTimeout(function(){$tr.removeClass('row-update')},4000);
    }
    this.update = function(plist){
      if(!plist)return;
      var c=this;
      var $t = this.$table;
      $.each(plist,function(i,v){c.updateProjectRow($t.find('#p'+i),v)})
      $t.trigger('sorton').trigger('update')
      this.updateTotals();
   }
  }
