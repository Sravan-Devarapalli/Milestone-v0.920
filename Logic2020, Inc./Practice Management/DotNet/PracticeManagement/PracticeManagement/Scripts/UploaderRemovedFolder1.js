/*
Uploadify v1.6.2 by RonnieSan
Copyright (C) 2009  Ronnie Garcia
Co-developed with Travis Nickels

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

import flash.external.ExternalInterface;
import flash.net.*;
import flash.events.*;
import flash.display.*;

// Create a new FileReference object
var param:Object = LoaderInfo(this.root.loaderInfo).parameters;
var fileRef:*;
var fileRefSingle:FileReference    = new FileReference();
var fileRefMulti:FileReferenceList = new FileReferenceList();
var fileRefListener:Object = new Object();
var fileQueue:Object       = new Object();
var activeQueue:Object     = new Object();
var cancelQueue:Object     = new Object();
var counter:Number        = 0;
var filesSelected:Number  = 0;
var filesReplaced:Number  = 0;
var filesUploaded:Number  = 0;
var filesChecked:Number   = 0;
var errors:Number         = 0;
var kbs:Number            = 0;
var allBytesLoaded:Number = 0;
var allBytesTotal:Number  = 0;
var allKbsAvg:Number      = 0;
var fileTypes;
stage.align = StageAlign.TOP_LEFT;
stage.scaleMode = StageScaleMode.NO_SCALE;

// Random string generator
function randomString(len:Number):String {
	var chars:Array = ['A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'];
	var rdmStr:String = '';
	var idx:Number;
	for (var i = 0; i < len; i++) {
		idx = Math.floor(Math.random() * 25);
		rdmStr += chars[idx];
	}
	return rdmStr;
}

function isFileNameHasSpecialChars(filename:String,validcharacters:String):String {
	//a-z   97-122
	//A-Z   65-90
	//space 32
	//0-9  48-57

    for (var i:int = 0; i < filename.length; i++) {
        var ch:Number = filename.charCodeAt(i);
        if (!((ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90) || (ch >= 48 && ch <= 57) || ch == 32 )) {
			if(!ContainsChar(validcharacters,filename.charAt(i).toString()))
            return filename.charAt(i).toString();
        }
    }
	return '';
}

function ContainsChar(str:String,ch:String):Boolean
{
	for (var i:int = 0; i < str.length; i++) {
        if(str.charAt(i).toString() == ch)
            return true;
    }
	return false;
}

// Load the button image
function setButtonImg():void {
	if (param.buttonImg) {
		var btnLoader:Loader = new Loader();
		browseBtn.addChild(btnLoader);
		var btnImage:URLRequest = new URLRequest(param.buttonImg);
		btnLoader.load(btnImage);
	}
	if (!param.hideButton && !param.buttonImg) {
		browseBtn.empty.alpha = 1;
	}
}
setButtonImg();

function setButtonText():void {
	if (param.buttonText) {
		browseBtn.empty.buttonText.text = unescape(param.buttonText);
	}
}
setButtonText();

browseBtn.buttonMode = true;
browseBtn.useHandCursor = true;
browseBtn.mouseChildren = false;

function setButtonSize():void {
	if (param.hideButton) {
		browseBtn.width  = param.btnWidth;
		browseBtn.height = param.btnHeight;
	}
	ExternalInterface.call('$("#' + param.fileUploadID + '").attr("width",' + param.btnWidth + ')');
	ExternalInterface.call('$("#' + param.fileUploadID + '").attr("height",' + param.btnHeight + ')');
}
setButtonSize();

if (param.rollover) {
	browseBtn.addEventListener(MouseEvent.ROLL_OVER, function (event:MouseEvent):void {
		event.currentTarget.y = -param.btnHeight;
	});
	browseBtn.addEventListener(MouseEvent.ROLL_OUT, function (event:MouseEvent):void {
		event.currentTarget.y = 0;
	});
	browseBtn.addEventListener(MouseEvent.MOUSE_DOWN, function (event:MouseEvent):void {
		event.currentTarget.y = -(param.btnHeight * 2);
	});
}

if (!param.scriptData) {
	param.scriptData = '';
}

// Upload a single file or multiple files
function setMulti():void {
	if (!param.multi) {
		fileRef = fileRefSingle;
	} else {
		fileRef = fileRefMulti;
	}
}
setMulti();

// Limit the file types
function setFileTypes():void {
	if (param.fileDesc && param.fileExt) {
		fileTypes = new FileFilter(param.fileDesc, param.fileExt);
	}
}
setFileTypes();

// Browse for Files
browseBtn.addEventListener(MouseEvent.CLICK, function():void {
	if (!fileTypes) {
		fileRef.browse();
	} else {
		fileRef.browse([fileTypes]);
	}
});

// Get the size of an object
function objSize(obj:Object):Number {
	var i:Number = 0;
	for (var item in obj) {
		i++;
	}
	return i;
}

// Get actual folder path
function getFolderPath():String {
	var folder:String = param.folder;
	if (param.folder.substr(0,1) != '/') {
		folder = param.pagepath + param.folder;
		var folderParts:Array = folder.split('/');
		for (var i = 0; i < folderParts.length; i++) {
			if (folderParts[i] == '..') {
				folderParts.splice(i - 1, 2);
			}
		}
		folder = folderParts.join('/');
	}
	return folder;
}

// When selecting a file
function fileSelectHandler(event:Event):void {
	var queueID:String = '';
	if (!param.multi) {
		fileQueue = new Object();
		queueID = randomString(6);
		fileQueue[queueID] = fileRef;
		ExternalInterface.call('$("#' + param.fileUploadID + 'Queue").empty()');
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuSelect",["' + queueID + '",{"name":"' + fileQueue[queueID].name + '","size":' + fileQueue[queueID].size + ',"creationDate":"' + fileQueue[queueID].creationDate + '","modificationDate":"' + fileQueue[queueID].modificationDate + '","type":"' + fileQueue[queueID].type + '"}])');
		filesSelected = 1;
	} else {
		var duplicate:Boolean = false;
		for (var i = 0; i < fileRef.fileList.length; i++) {
			for (var ID in fileQueue) {
				if (fileQueue[ID].name == fileRef.fileList[i].name) {
					duplicate = true;
					fileQueue[ID] = fileRef.fileList[i];
					filesReplaced++;
				}
			}
			if (!duplicate) {
				queueID = randomString(6);
				fileQueue[queueID] = fileRef.fileList[i];
				filesSelected++;
				allBytesTotal += fileQueue[queueID].size;
				ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuSelect",["' + queueID + '",{"name":"' + fileQueue[queueID].name + '","size":' + fileQueue[queueID].size + ',"creationDate":"' + fileQueue[queueID].creationDate + '","modificationDate":"' + fileQueue[queueID].modificationDate + '","type":"' + fileQueue[queueID].type + '"}])');
			}
			duplicate = false;
		}
	}
	ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuSelectOnce",[{"fileCount":' + objSize(fileQueue) + ',"filesSelected":' + filesSelected + ',"filesReplaced":' + filesReplaced + ',"allBytesTotal":' + allBytesTotal + '}])');
	filesSelected = 0;
	filesReplaced = 0;
	if (param.auto) {
		if (param.checkScript) {
			rfu_uploadFiles(null, false);
		} else {
			rfu_uploadFiles(null, false);
		}
	}
}
fileRef.addEventListener(Event.SELECT, fileSelectHandler);

function rfu_updateSettings(settingName:String, settingValue):void {
	param[settingName] = settingValue;
	if(settingName == 'buttonImg') setButtonImg();
	if(settingName == 'buttonText') setButtonText();
    if(settingName == 'fileDesc' || settingName == 'fileExt') setFileTypes();
	if(settingName == 'multi') setMulti();
	if(settingName == 'width' || settingName == 'height') setButtonSize();
}

function uploadCounter(event:Event):void {
	counter++;
}

// Start the upload manually
function rfu_uploadFiles(queueID:String, checkComplete:Boolean):void {
	if (param.checkScript && !checkComplete) {
		if (queueID) {
			ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuCheckExist", ["' + param.checkScript + '",{"' + queueID + '":"' + fileQueue[queueID].name + '"},"' + param.folder + '", true])');
		} else {
			var fileQueueString:String = '{';
			for (var fqID in fileQueue) {
				if (fileQueueString != '{') {
					fileQueueString += ',';
				}
				fileQueueString += '"' + fqID + '":"' + fileQueue[fqID].name + '"';
			}
			fileQueueString += '}';
			ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuCheckExist", ["' + param.checkScript + '",' + fileQueueString + ',"' + param.folder + '", false])');
		}
	} else {
		root.addEventListener(Event.ENTER_FRAME, uploadCounter);
		if (queueID) {
			if (fileQueue[queueID]) {
				uploadFile(fileQueue[queueID], queueID, true);
			}
		} else {
			if (param.simUploadLimit) {
				for (var activeID in fileQueue) {
					if (!activeQueue[activeID]) {
						if (objSize(activeQueue) < parseInt(param.simUploadLimit)) {
							uploadFile(fileQueue[activeID], activeID, false);
							activeQueue[activeID] = true;
						} else {
							break;
						}
					}
				}
			} else {
				for (var fileQueueID in fileQueue) {
					uploadFile(fileQueue[fileQueueID], fileQueueID, false);
				}
			}
		}
	}
}

// Upload each file
function uploadFile(file:FileReference, queueID:String, single:Boolean):void {
	var startTimer:Number = 0;
	var lastBytesLoaded:Number = 0;
	var kbsAvg:Number = 0;

	function fileOpenHandler(event:Event) {
		startTimer = getTimer();
	}

	function fileProgressHandler(event:ProgressEvent):void {
		var percentage:Number = Math.round((event.bytesLoaded / event.bytesTotal) * 100);
		if ((getTimer()-startTimer) >= 150) {
			kbs = ((event.bytesLoaded - lastBytesLoaded)/1024)/((getTimer()-startTimer)/1000);
			kbs = int(kbs*10)/10;
			startTimer = getTimer();
			if (kbsAvg > 0) {
				kbsAvg = (kbsAvg + kbs)/2;
			} else {
				kbsAvg = kbs;
			}
			allKbsAvg = (allKbsAvg + kbsAvg)/2;
		}
		allBytesLoaded += (event.bytesLoaded - lastBytesLoaded);
		lastBytesLoaded = event.bytesLoaded;
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuProgress",["' + queueID + '",{"name":"' + event.currentTarget.name + '","size":' + event.currentTarget.size + ',"creationDate":"' + event.currentTarget.creationDate + '","modificationDate":"' + event.currentTarget.modificationDate + '","type":"' + event.currentTarget.type + '"},{"percentage":' + percentage + ',"bytesLoaded":' + event.bytesLoaded + ',"allBytesLoaded":' + allBytesLoaded + ',"speed":' + kbs +'}])');
	}

	function fileCompleteHandler(event:DataEvent):void {
		if (kbsAvg == 0) {
			kbs = (file.size/1024)/((getTimer()-startTimer)/1000);
			kbsAvg = kbs;
			allKbsAvg = (allKbsAvg + kbsAvg)/2;
		}
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuProgress",["' + queueID + '",{"name":"' + event.currentTarget.name + '","size":' + event.currentTarget.size + ',"creationDate":"' + event.currentTarget.creationDate + '","modificationDate":"' + event.currentTarget.modificationDate + '","type":"' + event.currentTarget.type + '"},{"percentage":100,"bytesLoaded":' + event.currentTarget.size + ',"allBytesLoaded":' + allBytesLoaded + ',"speed":' + kbs + '}])');
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuComplete", ["' + queueID + '",{"name":"' + event.currentTarget.name + '","filePath":"' + getFolderPath() + '/' + event.currentTarget.name + '","size":' + event.currentTarget.size + ',"creationDate":"' + event.currentTarget.creationDate + '","modificationDate":"' + event.currentTarget.modificationDate + '","type":"' + event.currentTarget.type + '"},"' + escape(event.data) + '",{"fileCount":' + (objSize(fileQueue)-1) + ',"speed":' + kbsAvg + '}])');
		filesUploaded++;
		delete fileQueue[queueID];
		if (param.simUploadLimit && !single) {
			delete activeQueue[queueID];
			rfu_uploadFiles(null, true);
		}
		if (objSize(fileQueue) == 0) {
			ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuAllComplete",[{"filesUploaded":' + filesUploaded + ',"errors":' + errors + ',"allBytesLoaded":' + allBytesLoaded + ',"speed":' + allKbsAvg + '}])');
			root.removeEventListener(Event.ENTER_FRAME, uploadCounter);
			filesUploaded = 0;
			errors = 0;
			allBytesLoaded = 0;
			allBytesTotal = 0;
			allKbsAvg = 0;
			filesChecked = 0;
			event.currentTarget.removeEventListener(DataEvent.UPLOAD_COMPLETE_DATA, fileCompleteHandler);
		}
	}

	// Add all the event listeners
	file.addEventListener(Event.OPEN, fileOpenHandler);
	file.addEventListener(ProgressEvent.PROGRESS, fileProgressHandler);
	file.addEventListener(DataEvent.UPLOAD_COMPLETE_DATA, fileCompleteHandler);

	// Handle all the errors
	file.addEventListener(HTTPStatusEvent.HTTP_STATUS, function(event:HTTPStatusEvent):void {
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuError",["' + queueID + '",{"name":"' + event.currentTarget.name + '","size":' + event.currentTarget.size + ',"creationDate":"' + event.currentTarget.creationDate + '","modificationDate":"' + event.currentTarget.modificationDate + '","type":"' + event.currentTarget.type + '"},{"type":"HTTP","status":"' + event.status + '"}])');
		errors++;
		delete fileQueue[queueID];
	});
	file.addEventListener(IOErrorEvent.IO_ERROR, function(event:IOErrorEvent):void {
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuError",["' + queueID + '",{"name":"' + event.currentTarget.name + '","size":' + event.currentTarget.size + ',"creationDate":"' + event.currentTarget.creationDate + '","modificationDate":"' + event.currentTarget.modificationDate + '","type":"' + event.currentTarget.type + '"},{"type":"IO","text":"' + event.text + '"}])');
		errors++;
		delete fileQueue[queueID];
	});
	file.addEventListener(SecurityErrorEvent.SECURITY_ERROR, function(event:SecurityErrorEvent):void {
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuError",["' + queueID + '",{"name":"' + event.currentTarget.name + '","size":' + event.currentTarget.size + ',"creationDate":"' + event.currentTarget.creationDate + '","modificationDate":"' + event.currentTarget.modificationDate + '","type":"' + event.currentTarget.type + '"},{"type":"Security","text":"' + event.text + '"}])');
		errors++;
		delete fileQueue[queueID];
	});

	if (param.sizeLimit && file.size > parseInt(param.sizeLimit)) {
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuError",["' + queueID + '",{"name":"' + file.name + '","size":' + file.size + ',"creationDate":"' + file.creationDate + '","modificationDate":"' + file.modificationDate + '","type":"' + file.type + '"},{"type":"File Size","ErrorReason":' + param.sizeLimit + '}])');
		errors++;
		delete fileQueue[queueID];
	}
     else if (!param.allowSpecialChars && isFileNameHasSpecialChars(file.name,param.ValidChars) != '') {
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuError",["' + queueID + '",{"name":"' + file.name + '","size":' + file.size + ',"creationDate":"' + file.creationDate + '","modificationDate":"' + file.modificationDate + '","type":"' + file.type + '"},{"type":"File name voilation","ErrorReason":"' + isFileNameHasSpecialChars(file.name,param.ValidChars) + '"}])');
		errors++;
		delete fileQueue[queueID];
	}
    else {
		if (param.script.substr(0,1) != '/') param.script = param.pagepath + param.script;
		var scriptURL:URLRequest = new URLRequest(param.script + '?' + unescape(param.scriptData));
		file.upload(scriptURL, param.fileDataName);
	}
}

function rfu_cancelFileUpload(queueID:String):void {
	if (fileQueue[queueID]) {
		fileQueue[queueID].cancel();
		allBytesTotal -= fileQueue[queueID].size;
		ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuCancel", ["' + queueID + '",{"name":"' + fileQueue[queueID].name + '","size":' + fileQueue[queueID].size + ',"creationDate":"' + fileQueue[queueID].creationDate + '","modificationDate":"' + fileQueue[queueID].modificationDate + '","type":"' + fileQueue[queueID].type + '"},{"fileCount":' + (objSize(fileQueue)-1) + ',"allBytesTotal":' + allBytesTotal + '}])');
		delete fileQueue[queueID];
		if (param.simUploadLimit && activeQueue[queueID]) {
			delete activeQueue[queueID];
			rfu_uploadFiles(null, true);
		}
		root.removeEventListener(Event.ENTER_FRAME, uploadCounter);
	}
}

// Cancel all uploads
function rfu_clearFileUploadQueue():void {
	for (var queueID in fileQueue) {
		rfu_cancelFileUpload(queueID);
	}
	ExternalInterface.call('$("#' + param.fileUploadID + '").trigger("rfuClearQueue",[{"fileCount":' + objSize(fileQueue) + ',"allBytesTotal":' + allBytesTotal + '}])');
}

// Create all the callbacks for the functions
ExternalInterface.addCallback('updateSettings', rfu_updateSettings);
ExternalInterface.addCallback('startFileUpload', rfu_uploadFiles);
ExternalInterface.addCallback('cancelFileUpload', rfu_cancelFileUpload);
ExternalInterface.addCallback('clearFileUploadQueue', rfu_clearFileUploadQueue);
