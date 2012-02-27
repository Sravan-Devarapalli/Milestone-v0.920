Type.registerNamespace( "PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection" );

PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior = function ( element ) {
    PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.initializeBase( this, [element] );
    this.controlToDisableId = null;
    this._isControlWasDisabled = false;
}

PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.callBaseMethod( this, 'initialize' );

        var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
        for ( var i = 0; i < hoursControlIds.length; i++ ) {
            var control = document.getElementById( hoursControlIds[i] );
            if ( control ) {
                $addHandlers( 
                     control,
                     {
                         'change': this._onChange
                     },
                     this );
            }
        }

        var notesControlIds = this.getControlIdList( this.notesControlsToCheck );
        for ( var i = 0; i < notesControlIds.length; i++ ) {
            var control = document.getElementById( notesControlIds[i] );
            if ( control ) {
                $addHandlers( 
                     control,
                     {
                         'change': this._onChange
                     },
                     this );
            }
        }
        var deleteControlIds = this.getControlIdList( this.deleteControlsToCheck );
        for ( var i = 0; i < deleteControlIds.length; i++ ) {
            var control = document.getElementById( deleteControlIds[i] );
            if ( control ) {
                $addHandlers( 
                     control,
                     {
                         'mousedown': this._onMousedown
                     },
                     this );
            }
        }

        var closeControlIds = this.getControlIdList( this.closeControlsToCheck );
        for ( var i = 0; i < closeControlIds.length; i++ ) {
            var control = document.getElementById( closeControlIds[i] );
            if ( control ) {
                $addHandlers( 
                     control,
                     {
                         'click': this._onCloseClick
                     },
                     this );
            }
        }


        this._onChange();
    },

    dispose: function () {
        // Cleanup code 
        PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.callBaseMethod( this, 'dispose' );
    },

    getControlIdList: function ( controlsToCheck ) {
        var contolsToCheckList = controlsToCheck;
        if ( contolsToCheckList )
            return contolsToCheckList.split( ';' );
        else
            return [];
    },
    isHoursControlId: function ( targetId ) {
        var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
        for ( var i = 0; i < hoursControlIds.length; i++ ) {
            if ( hoursControlIds[i] == targetId ) {
                return true;
            }
        }
        return false;
    },
    getTargetControl: function () {
        var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
        for ( var i = 0; i < hoursControlIds.length; i++ ) {
            var targetAcutalHours = document.getElementById( hoursControlIds[i] );
            if ( targetAcutalHours != null && targetAcutalHours.value != '' ) {
                return targetAcutalHours;
            }
        }
        var notesControlIds = this.getControlIdList( this.notesControlsToCheck );
        for ( var i = 0; i < notesControlIds.length; i++ ) {
            var targetNotes = document.getElementById( notesControlIds[i] );
            if ( targetNotes != null && targetNotes.value != '' ) {
                return targetNotes;
            }
        }
        return null;
    },
    getTargetControlIndex: function ( targetId ) {
        var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
        for ( var i = 0; i < hoursControlIds.length; i++ ) {
            if ( hoursControlIds[i] == targetId ) {
                return i;
            }
        }
        var notesControlIds = this.getControlIdList( this.notesControlsToCheck );
        for ( var i = 0; i < notesControlIds.length; i++ ) {
            var targetNotes = document.getElementById( notesControlIds[i] );
            if ( notesControlIds[i] == targetId ) {
                return i;
            }
        }
        return null;
    },
    _onMousedown: function () {
        var target = this.getTargetControl();
        if ( target != null ) {
            this.enableAllColtrols();
        }
    },
    disableOtherColtrols: function ( index ) {
        var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
        for ( var i = 0; i < hoursControlIds.length; i++ ) {
            var isReviewed = false;
            var targetAcutalHours = document.getElementById( hoursControlIds[i] );
            if ( targetAcutalHours != null ) {
                if ( targetAcutalHours.getAttribute( 'IsReviewed' ) != undefined && targetAcutalHours.getAttribute( 'IsReviewed' ) != null ) {
                    isReviewed = targetAcutalHours.getAttribute( 'IsReviewed' ).toLowerCase() == 'approved';
                }
                if ( !isReviewed ) {
                    if ( i != index )
                        targetAcutalHours.setAttribute( 'readonly', 'readonly' );
                    else
                        targetAcutalHours.removeAttribute( 'readonly' );
                } else {
                    targetAcutalHours.setAttribute( 'disabled', 'disabled' );
                }
            }
        }
        var notesControlIds = this.getControlIdList( this.notesControlsToCheck );
        for ( var i = 0; i < notesControlIds.length; i++ ) {
            var targetNotes = document.getElementById( notesControlIds[i] );
            if ( targetNotes != null ) {
                if ( i != index )
                    targetNotes.setAttribute( 'readonly', 'readonly' );
                else
                    targetNotes.removeAttribute( 'readonly' );
            }
        }
        var deleteControlIds = this.getControlIdList( this.deleteControlsToCheck );
        for ( var i = 0; i < deleteControlIds.length; i++ ) {
            var targetDelete = document.getElementById( deleteControlIds[i] );
            if ( targetDelete != null ) {
                if ( i != index )
                    targetDelete.setAttribute( 'disabled', 'disabled' );
                else
                    targetDelete.removeAttribute( 'disabled' );
            }
        }


    },
    enableAllColtrols: function () {
        var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
        for ( var i = 0; i < hoursControlIds.length; i++ ) {
            var isReviewed = false;
            var targetAcutalHours = document.getElementById( hoursControlIds[i] );
            if ( targetAcutalHours != null ) {
                if ( targetAcutalHours.getAttribute( 'IsReviewed' ) != undefined && targetAcutalHours.getAttribute( 'IsReviewed' ) != null ) {
                    isReviewed = targetAcutalHours.getAttribute( 'IsReviewed' ).toLowerCase() == 'approved';
                }
                if ( !isReviewed ) {
                    targetAcutalHours.removeAttribute( 'readonly' );
                }
                else {
                    targetAcutalHours.setAttribute( 'disabled', 'disabled' );
                }
            }
        }
        var notesControlIds = this.getControlIdList( this.notesControlsToCheck );
        for ( var i = 0; i < notesControlIds.length; i++ ) {
            var targetNotes = document.getElementById( notesControlIds[i] );
            if ( targetNotes != null ) {
                targetNotes.removeAttribute( 'readonly' );
            }
        }
        var deleteControlIds = this.getControlIdList( this.deleteControlsToCheck );
        for ( var i = 0; i < deleteControlIds.length; i++ ) {
            var targetDelete = document.getElementById( deleteControlIds[i] );
            if ( targetDelete != null ) {
                targetDelete.removeAttribute( 'disabled' );
            }
        }
    },
    _onCloseClick: function () {
        
        var target = this.getTargetControl();
        if ( target != null ) {
            var index = this.getTargetControlIndex( target.id );
            var hiddenNotesControlIds = this.getControlIdList( this.hiddenNotesControlsToCheck );
            var hoursControlIds = this.getControlIdList( this.hoursControlsToCheck );
            var hiddenNotesControl = document.getElementById( hiddenNotesControlIds[index] );
            var hoursControl = document.getElementById( hoursControlIds[index] );
            if ( hiddenNotesControl != null && hoursControl != null ) {
                if ( hiddenNotesControl.value == '' && hoursControl.value == '' ) {
                    //notes not save roll back the onchange event and enable all  
                    this.enableAllColtrols();
                }
            }
        }

    },
    _onChange: function () {

        var target = this.getTargetControl();
        if ( target != null ) {

            var isHourTextBox = this.isHoursControlId( target.id );
            var targetAcutalHoursHiddenField = document.getElementById( this.targetAcutalHoursHiddenFieldId );
            var targetNotesHiddenField = document.getElementById( this.targetNotesHiddenFieldId );

            var targetAcutalHours = null;
            var targetNotes = null;

            if ( targetAcutalHoursHiddenField.value != '' )
                var targetAcutalHours = document.getElementById( targetAcutalHoursHiddenField.value );

            if ( targetNotesHiddenField.value != '' )
                var targetNotes = document.getElementById( targetNotesHiddenField.value );

            if ( ( targetAcutalHours == null || targetAcutalHours == undefined ) && isHourTextBox ) {
                targetAcutalHoursHiddenField.value = target.id;
                targetAcutalHours = document.getElementById( target.id );
            }
            if ( ( targetNotes == null || targetNotes == undefined ) && !isHourTextBox ) {
                targetNotesHiddenField.value = target.id;
                targetNotes = document.getElementById( target.id );
            }

            if ( targetAcutalHours != null || targetNotes != null ) {
                var index = this.getTargetControlIndex( target.id );
                this.disableOtherColtrols( index );
            }
        }
    }

}

PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.registerClass( 'PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior', AjaxControlToolkit.BehaviorBase );

