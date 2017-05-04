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
