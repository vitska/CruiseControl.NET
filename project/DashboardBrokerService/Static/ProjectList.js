  function ProjectList(selector){
    var $sel=$(selector)
    this.$panel = $sel
    this.$header = $sel.append('<div class="panel-heading"><h3 class="panel-title">Projects</h3></div>')
    $sel.append('<table class="table table-bordered project-table tablesorter">'+
                '<thead>'+
                '  <tr>'+
                '    <th style="width: 1em;">&nbsp;</th>'+
                '    <th>Server<i class="fa"></i></th>'+
                '    <th>Project<i class="fa"></i></th>'+
                '    <th>Build<i class="fa"></i></th>'+
                '    <th>Build time<i class="fa"></i></th>'+
                '    <th>Next<i class="fa"></i></th>'+
                '    <th>Label<i class="fa"></i></th>'+
                '    <th>CC<i class="fa"></i></th>'+
                '    <th>Activity<i class="fa"></i></th>'+
                '    <th>'+
                '      <button type="button" class="btn btn-xs btn-primary cmd-group cmd-abort">Abort</button>'+
                '      <button type="button" class="btn btn-xs btn-warning cmd-force">Force</button>'+
                '      <button type="button" class="btn btn-xs btn-success cmd-start">Start</button>'+
                '      <button type="button" class="btn btn-xs btn-danger cmd-stop">Stop</button></th>'+
                '  </tr>'+
                '</thead>'+
                '<tbody></tbody>'+
                '</table>');
    this.$table = $sel.find('table.project-table')
    setInterval(function(){
      $('.time-update').each(function(){
        $(this).text(moment.duration(moment() - moment($(this).data('time'))).format())//.addClass('time-update').data('time', d.nb)
      //debugger;
      })
      $('.time-progress-update').each(function(){
        $(this).find('.progress-bar').text(moment.duration(moment() - moment($(this).data('time'))).format())//.addClass('time-update').data('time', d.nb)
      //debugger;
      })
    },1000);
    this.dcell = function(cls){
      return $('<td class="col">').addClass(cls);
    }
    this.render = function(srv){
      if(!srv.projects)return;
      var c=this;
      c.$table.find('tbody').append(
        $.map(srv.projects, function(v,i){
          return $('<tr>').attr('id', 'p'+i).addClass('s'+srv.id).data('proj',{srv:srv, prjid:i, name:v.name}).append(
            c.dcell('col-row-state').append('<i class="fa fa-caret-right"></i>')
            ,c.dcell('col-sever').text(srv.name)
            ,c.dcell('col-prj-name').text(v.name)
            ,c.dcell('col-build-status')
            ,c.dcell('col-prj-time')
            ,c.dcell('col-prj-next col-pbar')
            ,c.dcell('col-prj-label')
            ,c.dcell('col-cc-status')
            ,c.dcell('col-activity')
            ,c.dcell('col-buttons').append(
              '<button type="button" item-cmd="abort" class="btn btn-xs btn-primary cmd-btn cmd-abort">Abort</button>'+
              '<button type="button" item-cmd="force" class="btn btn-xs btn-warning cmd-btn cmd-force">Force</button>'+
              '<button type="button" item-cmd="start" class="btn btn-xs btn-success cmd-btn cmd-start">Start</button>'+
              '<button type="button" item-cmd="stop" class="btn btn-xs btn-danger cmd-btn cmd-stop">Stop</button>'
            )
          )
        })
      )
      var h = c.$header.find('h3')
      var p = c.$panel;
      c.renderHdrVal(h,'total','plug',function(){p.removeClass('show-fail show-success show-running show-building')})
      c.renderHdrVal(h,'success','check-circle',function(){p.toggleClass('show-success')})
      c.renderHdrVal(h,'fail','times-circle',function(){p.toggleClass('show-fail')})
      c.renderHdrVal(h,'building','gears',function(){p.toggleClass('show-building')})
      c.renderHdrVal(h,'sleep','moon-o',function(){})
      c.renderHdrVal(h,'running','play',function(){p.toggleClass('show-running')})
      c.$table.tablesorter()
      c.updateTotals();
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
      var c=this,h=c.$header,t=c.$table;
      var totals = {
        total:t.find('tbody tr').length,
        success:t.find('tbody tr.bs-success').length,
        fail:t.find('tbody tr.bs-fail').length,
        building:t.find('tbody tr.caa-building').length,
        sleeping:t.find('tbody tr.caa-sleep').length,
        running:t.find('tbody tr.ccs-running').length
      }
      h.find('.total .value').text(totals.total)
      h.find('.success .value').text(totals.success)
      h.find('.fail .value').text(totals.fail)
      h.find('.building .value').text(totals.building)
      h.find('.sleep .value').text(totals.sleeping)
      h.find('.running .value').text(totals.running)
      document.title = 'B'+t.find('tbody tr.caa-building').length + ' F'+ totals.fail+' S'+totals.success + ' T'+totals.total
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
      //Recalculate time
      d.nb = (d.srv.dtb + v.nb) * 1000;
      $tr.data('proj',d)
      $tr.find('.col-prj-time').text(d.abt)
      c.showBuildStatus($tr,d)
      c.showCCStatus($tr,d)
      c.showCCActivity($tr,d)
      $tr.find('.col-prj-next').html($('<div>').append(
//        .text(moment.duration(moment() - moment(d.nb)).format())
        //$('<span class="time-update">').data('time', d.nb)
        $('<div class="progress time-progress-update">')
          .append('<div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%"><span class="sr-only">45% Complete</span></div>')
          .data('time', d.nb)
      ));
      $tr.find('.col-prj-label').text(d.lb)
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
