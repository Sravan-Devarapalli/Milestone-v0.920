﻿/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.themes.add('default',(function(){var a={};function b(c,d){var e,f;f=c.config.sharedSpaces;f=f&&f[d];f=f&&CKEDITOR.document.getById(f);if(f){var g='<span class="cke_shared " dir="'+c.lang.dir+'"'+'>'+'<span class="'+c.skinClass+' '+c.id+' cke_editor_'+c.name+'">'+'<span class="'+CKEDITOR.env.cssClass+'">'+'<span class="cke_wrapper cke_'+c.lang.dir+'">'+'<span class="cke_editor">'+'<div class="cke_'+d+'">'+'</div></span></span></span></span></span>',h=f.append(CKEDITOR.dom.element.createFromHtml(g,f.getDocument()));if(f.getCustomData('cke_hasshared'))h.hide();else f.setCustomData('cke_hasshared',1);e=h.getChild([0,0,0,0]);!c.sharedSpaces&&(c.sharedSpaces={});c.sharedSpaces[d]=e;c.on('focus',function(){for(var i=0,j,k=f.getChildren();j=k.getItem(i);i++){if(j.type==CKEDITOR.NODE_ELEMENT&&!j.equals(h)&&j.hasClass('cke_shared'))j.hide();}h.show();});c.on('destroy',function(){h.remove();});}return e;};return{build:function(c,d){var e=c.name,f=c.element,g=c.elementMode;if(!f||g==CKEDITOR.ELEMENT_MODE_NONE)return;if(g==CKEDITOR.ELEMENT_MODE_REPLACE)f.hide();var h=c.fire('themeSpace',{space:'top',html:''}).html,i=c.fire('themeSpace',{space:'contents',html:''}).html,j=c.fireOnce('themeSpace',{space:'bottom',html:''}).html,k=i&&c.config.height,l=c.config.tabIndex||c.element.getAttribute('tabindex')||0;if(!i)k='auto';else if(!isNaN(k))k+='px';var m='',n=c.config.width;if(n){if(!isNaN(n))n+='px';m+='width: '+n+';';}var o=h&&b(c,'top'),p=b(c,'bottom');o&&(o.setHtml(h),h='');p&&(p.setHtml(j),j='');var q='<style>.'+c.skinClass+'{visibility:hidden;}</style>';if(a[c.skinClass])q='';else a[c.skinClass]=1;var r=CKEDITOR.dom.element.createFromHtml(['<span id="cke_',e,'" class="',c.skinClass,' ',c.id,' cke_editor_',e,'" dir="',c.lang.dir,'" title="',CKEDITOR.env.gecko?' ':'','" lang="',c.langCode,'"'+(CKEDITOR.env.webkit?' tabindex="'+l+'"':'')+' role="application"'+' aria-labelledby="cke_',e,'_arialbl"'+(m?' style="'+m+'"':'')+'>'+'<span id="cke_',e,'_arialbl" class="cke_voice_label">'+c.lang.editor+'</span>'+'<span class="',CKEDITOR.env.cssClass,'" role="presentation"><span class="cke_wrapper cke_',c.lang.dir,'" role="presentation"><table class="cke_editor" border="0" cellspacing="0" cellpadding="0" role="presentation"><tbody><tr',h?'':' style="display:none"',' role="presentation"><td id="cke_top_',e,'" class="cke_top" role="presentation">',h,'</td></tr><tr',i?'':' style="display:none"',' role="presentation"><td id="cke_contents_',e,'" class="cke_contents" style="height:',k,'" role="presentation">',i,'</td></tr><tr',j?'':' style="display:none"',' role="presentation"><td id="cke_bottom_',e,'" class="cke_bottom" role="presentation">',j,'</td></tr></tbody></table>'+q+'</span>'+'</span>'+'</span>'].join(''));
r.getChild([1,0,0,0,0]).unselectable();r.getChild([1,0,0,0,2]).unselectable();if(g==CKEDITOR.ELEMENT_MODE_REPLACE)r.insertAfter(f);else f.append(r);c.container=r;r.disableContextMenu();c.on('contentDirChanged',function(s){var t=(c.lang.dir!=s.data?'add':'remove')+'Class';r.getChild(1)[t]('cke_mixed_dir_content');var u=this.sharedSpaces&&this.sharedSpaces[this.config.toolbarLocation];u&&u.getParent().getParent()[t]('cke_mixed_dir_content');});c.fireOnce('themeLoaded');c.fireOnce('uiReady');},buildDialog:function(c){var d=CKEDITOR.tools.getNextNumber(),e=CKEDITOR.dom.element.createFromHtml(['<div class="',c.id,'_dialog cke_editor_',c.name.replace('.','\\.'),'_dialog cke_skin_',c.skinName,'" dir="',c.lang.dir,'" lang="',c.langCode,'" role="dialog" aria-labelledby="%title#"><table class="cke_dialog',' '+CKEDITOR.env.cssClass,' cke_',c.lang.dir,'" style="position:absolute" role="presentation"><tr><td role="presentation"><div class="%body" role="presentation"><div id="%title#" class="%title" role="presentation"></div><a id="%close_button#" class="%close_button" href="javascript:void(0)" title="'+c.lang.common.close+'" role="button"><span class="cke_label">X</span></a>'+'<div id="%tabs#" class="%tabs" role="tablist"></div>'+'<table class="%contents" role="presentation">'+'<tr>'+'<td id="%contents#" class="%contents" role="presentation"></td>'+'</tr>'+'<tr>'+'<td id="%footer#" class="%footer" role="presentation"></td>'+'</tr>'+'</table>'+'</div>'+'<div id="%tl#" class="%tl"></div>'+'<div id="%tc#" class="%tc"></div>'+'<div id="%tr#" class="%tr"></div>'+'<div id="%ml#" class="%ml"></div>'+'<div id="%mr#" class="%mr"></div>'+'<div id="%bl#" class="%bl"></div>'+'<div id="%bc#" class="%bc"></div>'+'<div id="%br#" class="%br"></div>'+'</td></tr>'+'</table>',CKEDITOR.env.ie?'':'<style>.cke_dialog{visibility:hidden;}</style>','</div>'].join('').replace(/#/g,'_'+d).replace(/%/g,'cke_dialog_')),f=e.getChild([0,0,0,0,0]),g=f.getChild(0),h=f.getChild(1);if(CKEDITOR.env.ie&&!CKEDITOR.env.ie6Compat){var i=CKEDITOR.env.isCustomDomain(),j='javascript:void(function(){'+encodeURIComponent('document.open();'+(i?'document.domain="'+document.domain+'";':'')+'document.close();')+'}())',k=CKEDITOR.dom.element.createFromHtml('<iframe frameBorder="0" class="cke_iframe_shim" src="'+j+'"'+' tabIndex="-1"'+'></iframe>');k.appendTo(f.getParent());}g.unselectable();h.unselectable();return{element:e,parts:{dialog:e.getChild(0),title:g,close:h,tabs:f.getChild(2),contents:f.getChild([3,0,0,0]),footer:f.getChild([3,0,1,0])}};
},destroy:function(c){var d=c.container,e=c.element;if(d){d.clearCustomData();d.remove();}if(e){e.clearCustomData();c.elementMode==CKEDITOR.ELEMENT_MODE_REPLACE&&e.show();delete c.element;}}};})());CKEDITOR.editor.prototype.getThemeSpace=function(a){var b='cke_'+a,c=this._[b]||(this._[b]=CKEDITOR.document.getById(b+'_'+this.name));return c;};CKEDITOR.editor.prototype.resize=function(a,b,c,d){var j=this;var e=j.container,f=CKEDITOR.document.getById('cke_contents_'+j.name),g=CKEDITOR.env.webkit&&j.document&&j.document.getWindow().$.frameElement,h=d?e.getChild(1):e;h.setSize('width',a,true);g&&(g.style.width='1%');var i=c?0:(h.$.offsetHeight||0)-(f.$.clientHeight||0);f.setStyle('height',Math.max(b-i,0)+'px');g&&(g.style.width='100%');j.fire('resize');};CKEDITOR.editor.prototype.getResizable=function(a){return a?CKEDITOR.document.getById('cke_contents_'+this.name):this.container;};

