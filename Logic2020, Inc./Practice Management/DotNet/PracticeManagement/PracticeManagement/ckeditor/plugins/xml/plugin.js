﻿/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

(function(){CKEDITOR.plugins.add('xml',{});CKEDITOR.xml=function(a){var b=null;if(typeof a=='object')b=a;else{var c=(a||'').replace(/&nbsp;/g,'\xa0');if(window.DOMParser)b=new DOMParser().parseFromString(c,'text/xml');else if(window.ActiveXObject){try{b=new ActiveXObject('MSXML2.DOMDocument');}catch(d){try{b=new ActiveXObject('Microsoft.XmlDom');}catch(d){}}if(b){b.async=false;b.resolveExternals=false;b.validateOnParse=false;b.loadXML(c);}}}this.baseXml=b;};CKEDITOR.xml.prototype={selectSingleNode:function(a,b){var c=this.baseXml;if(b||(b=c))if(CKEDITOR.env.ie||b.selectSingleNode)return b.selectSingleNode(a);else if(c.evaluate){var d=c.evaluate(a,b,null,9,null);return d&&d.singleNodeValue||null;}return null;},selectNodes:function(a,b){var c=this.baseXml,d=[];if(b||(b=c))if(CKEDITOR.env.ie||b.selectNodes)return b.selectNodes(a);else if(c.evaluate){var e=c.evaluate(a,b,null,5,null);if(e){var f;while(f=e.iterateNext())d.push(f);}}return d;},getInnerXml:function(a,b){var c=this.selectSingleNode(a,b),d=[];if(c){c=c.firstChild;while(c){if(c.xml)d.push(c.xml);else if(window.XMLSerializer)d.push(new XMLSerializer().serializeToString(c));c=c.nextSibling;}}return d.length?d.join(''):null;}};})();

