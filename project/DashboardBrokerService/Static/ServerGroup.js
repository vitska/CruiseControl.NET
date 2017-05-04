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
