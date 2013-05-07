<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:dt="urn:schemas-microsoft-com:datatypes">

  <xsl:param name="currentUrl" />

  <xsl:output method="html" indent="no"/>
  <xsl:variable name="root" select="node()"/>
  <xsl:variable name="rootName" select="name($root)" />
  <xsl:variable name="redirectUrl">
    <xsl:text>&amp;returnTo=</xsl:text>
    <xsl:value-of select="$currentUrl"/>
  </xsl:variable>
  <xsl:template match="/">
    <xsl:apply-templates select="//NEW_VALUES" mode="list"></xsl:apply-templates>
  </xsl:template>

  <xsl:template match="NEW_VALUES" mode="list">
    <xsl:choose>
      <xsl:when test="$rootName = 'Note'">
        <xsl:call-template name="NEW_VALUES_NOTES"></xsl:call-template>
      </xsl:when>
      <xsl:when test="$rootName = 'Export'">
        <xsl:call-template name="NEW_VALUES_EXPORT"></xsl:call-template>
      </xsl:when>
      <xsl:when test="count(./OLD_VALUES/attribute::*) = 0">
        <xsl:apply-templates select="." mode="insert_delete"></xsl:apply-templates>
      </xsl:when>
      <xsl:when test="count(./attribute::*) = 0">
        <xsl:apply-templates select="OLD_VALUES" mode="insert_delete"></xsl:apply-templates>
      </xsl:when>
      <!--<xsl:when test="$rootName = 'Milestone'">
        <xsl:apply-templates select="." mode="insert_delete"></xsl:apply-templates>
      </xsl:when>-->
      <!--<xsl:when test="$rootName = 'ProjectAttachment'">
        <xsl:apply-templates select="." mode="insert_delete"></xsl:apply-templates>
      </xsl:when>-->
      <!--<xsl:when test="$rootName = 'Opportunity'">
        <xsl:apply-templates select="." mode="insert_delete"></xsl:apply-templates>
      </xsl:when>-->
      <xsl:otherwise>
        <xsl:apply-templates select="." mode="update"></xsl:apply-templates>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="NEW_VALUES" mode="update">

    <xsl:for-each select="./attribute::*">
      <xsl:variable name="value" select="." />
      <xsl:variable name="attrName" select="name()" />

      <xsl:if test="$attrName = 'SkillsDescription' or $attrName = 'IndustryDescription' or $attrName = 'Person' or $attrName = 'SkillCategory' or $attrName = 'SkillType' or $attrName = 'ProfileName' or $attrName = 'Practice'">
        <xsl:call-template name="FriendlyName">
          <xsl:with-param name="attrName" select="name()" />
        </xsl:call-template>:
        <xsl:call-template name="DisplayValue"></xsl:call-template>
        <br/>
      </xsl:if>

      <xsl:choose>
        <xsl:when test="$attrName = 'ProjectId' or $attrName = 'ObjectPersonId' or $attrName = 'TimeEntryId' or $attrName = 'ClientId' or $attrName = 'PersonId' 
                  or $attrName = 'ModifiedByPersonId' or $attrName = 'ModifiedBy' or $attrName = 'MilestoneId' or $attrName = 'MilestoneProjectId'
                  or $attrName = 'MilestonePersonId' or $attrName = 'Id' or $attrName = 'ModifiedByName' or $attrName = 'TimeTypeId' or $attrName = 'OpportunityId'
                  or $attrName = 'SalespersonId' or $attrName = 'PracticeId' or $attrName = 'OpportunityStatusId' or $attrName = 'OwnerId' or $attrName = 'GroupId'
                  or $attrName = 'LastUpdated' or $attrName = 'Tag' or $attrName = 'OpportunityTransitionStatusId' or $attrName='ProjectStatusId' or $attrName = 'ProjectManagerId'
                  or $attrName = 'DirectorId' or $attrName = 'User' or $attrName = 'PracticeManagerId' or $attrName = 'ProjectGroupId' or $attrName = 'IsAllowedToShow'
                  or $attrName = 'DivisionId' or $attrName = 'AccountId' or $attrName = 'DefaultDirectorID' or $attrName = 'DefaultSalespersonID' or $attrName = 'TerminationReasonId'
                  or $attrName = 'ProfileId'  or $attrName = 'ProjectOwnerId' or $attrName = 'PracticeCapabilityId' or $attrName = 'OpportunityTransitionId' or $attrName = 'PriorityId' 
                  or $attrName = 'ManagerId' or $attrName = 'SeniorityId' or $attrName = 'RecruiterId' or $attrName = 'TitleId' or $attrName = 'TitleTypeId' or $attrName = 'PricingListId' 
                  or $attrName = 'BusinessUnitId' or $attrName = 'BusinessGroupId' or $attrName = 'CSATId' or $attrName = 'ReviewerId' or $attrName = 'SeniorManagerId' or $attrName = 'BusinessTypeId'"></xsl:when>
        <xsl:otherwise>
          <xsl:for-each select="parent::*/OLD_VALUES/attribute::*">
            <xsl:if test="name() = $attrName and . != $value">
              <xsl:call-template name="DisplayChange">
                <xsl:with-param name="attrName" select="name()" />
                <xsl:with-param name="newValue" select="$value" />
                <xsl:with-param name="oldValue" select="." />
              </xsl:call-template>
            </xsl:if>
          </xsl:for-each>

          <xsl:if test="not(parent::*/OLD_VALUES/attribute::*[name() = $attrName])">
            <xsl:call-template name="DisplayChange">
              <xsl:with-param name="attrName" select="name()" />
              <xsl:with-param name="newValue" select="$value" />
              <xsl:with-param name="oldValue" />
            </xsl:call-template>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>

    <xsl:for-each select="./OLD_VALUES/attribute::*">
      <xsl:variable name="value" select="." />
      <xsl:variable name="attrName" select="name()" />

      <xsl:choose>
        <xsl:when test="$attrName = 'ProjectId' or $attrName = 'ObjectPersonId' or $attrName = 'TimeEntryId' or $attrName = 'ClientId' or $attrName = 'PersonId' 
                  or $attrName = 'ModifiedByPersonId' or $attrName = 'ModifiedBy' or $attrName = 'MilestoneId' or $attrName = 'MilestoneProjectId'
                  or $attrName = 'MilestonePersonId' or $attrName = 'Id' or $attrName = 'ModifiedByName' or $attrName = 'TimeTypeId' or $attrName = 'OpportunityId'
                  or $attrName = 'SalespersonId' or $attrName = 'PracticeId' or $attrName = 'OpportunityStatusId' or $attrName = 'OwnerId' or $attrName = 'GroupId'
                  or $attrName = 'LastUpdated' or $attrName = 'Tag' or $attrName = 'OpportunityTransitionStatusId' or $attrName='ProjectStatusId' or $attrName = 'ProjectManagerId'
                  or $attrName = 'DirectorId' or $attrName = 'User' or $attrName = 'PracticeManagerId' or $attrName = 'ProjectGroupId' or $attrName = 'IsAllowedToShow'
                  or $attrName = 'DivisionId' or $attrName = 'AccountId' or $attrName = 'DefaultDirectorID' or $attrName = 'DefaultSalespersonID' or $attrName = 'TerminationReasonId'
                  or $attrName = 'ProfileId' or $attrName = 'ProjectOwnerId' or $attrName = 'PracticeCapabilityId' or $attrName = 'OpportunityTransitionId' or $attrName = 'PriorityId' 
                  or $attrName = 'ManagerId' or $attrName = 'SeniorityId' or $attrName = 'RecruiterId' or $attrName = 'TitleId' or $attrName = 'TitleTypeId' or $attrName = 'PricingListId' 
                  or $attrName = 'BusinessUnitId' or $attrName = 'BusinessGroupId'  or $attrName = 'CSATId' or $attrName = 'ReviewerId' or $attrName = 'SeniorManagerId' or $attrName = 'BusinessTypeId'"></xsl:when>
        <xsl:otherwise>
          <xsl:if test="not(parent::*/parent::*/attribute::*[name() = $attrName])">
            <xsl:call-template name="DisplayChange">
              <xsl:with-param name="attrName" select="name()" />
              <xsl:with-param name="newValue" ></xsl:with-param>
              <xsl:with-param name="oldValue" select="." />
            </xsl:call-template>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="NEW_VALUES_NOTES">
    <xsl:for-each select="./attribute::*[name() = 'NoteText' or name() = 'NoteTargetName' or name() = 'NoteAddedTo']">
      <xsl:variable name="value" select="." />
      <xsl:variable name="attrName" select="name()" />

      <xsl:call-template name="FriendlyName">
        <xsl:with-param name="attrName" select="name()" />
      </xsl:call-template>:
      <b>
        <xsl:choose>
          <!--<xsl:when test="$attrName = 'By'">
            <xsl:call-template name="DisplayRedirect">
              <xsl:with-param name="needHyperlink" select="'true'" />
            </xsl:call-template>
          </xsl:when>-->
          <xsl:when test="$attrName = 'NoteText'">
            <xsl:call-template name="DisplayValue"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="DisplayValue"/>
          </xsl:otherwise>
        </xsl:choose>
        <!--<xsl:if test="$attrName = 'By'">
          <xsl:call-template name="DisplayRedirect">
            <xsl:with-param name="needHyperlink" select="'true'" />
          </xsl:call-template>
        </xsl:if>
        <xsl:if test="$attrName = 'NoteText'">
          <xsl:call-template name="DisplayValue"/>
        </xsl:if>-->
      </b>
      <xsl:element name="br"></xsl:element>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="NEW_VALUES_EXPORT">
    <xsl:for-each select="./attribute::*[name() = 'From']">
      <xsl:value-of select="name()"/>:&#160;
      <b>
        <xsl:value-of select="."/>
      </b>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="NEW_VALUES | OLD_VALUES" mode="insert_delete">
    <xsl:variable name="isNew" select="name() = 'NEW_VALUES' and count(./attribute::*) > 0" />
    <xsl:for-each select="./attribute::*">
      <xsl:variable name="attrName" select="name()"/>
      <xsl:choose>
        <xsl:when test="$attrName = 'ProjectId' or $attrName = 'ObjectPersonId' or $attrName = 'TimeEntryId' or $attrName = 'ClientId' or $attrName = 'PersonId' 
                  or $attrName = 'ModifiedByPersonId' or $attrName = 'ModifiedBy' or $attrName = 'MilestoneId' or $attrName = 'MilestoneProjectId'
                  or $attrName = 'MilestonePersonId' or $attrName = 'Id' or $attrName = 'ModifiedByName' or $attrName = 'TimeTypeId' or $attrName = 'OpportunityId'
                  or $attrName = 'SalespersonId' or $attrName = 'PracticeId' or $attrName = 'OpportunityStatusId' or $attrName = 'OwnerId' or $attrName = 'GroupId'
                  or $attrName = 'LastUpdated' or $attrName = 'Tag' or $attrName = 'OpportunityTransitionStatusId' or $attrName='ProjectStatusId' or $attrName = 'ProjectManagerId'
                  or $attrName = 'DirectorId' or $attrName = 'User' or $attrName = 'PracticeManagerId' or $attrName = 'ProjectGroupId' or $attrName = 'IsAllowedToShow'
                  or $attrName = 'DivisionId' or $attrName = 'AccountId' or $attrName = 'DefaultDirectorID' or $attrName = 'DefaultSalespersonID' or $attrName = 'TerminationReasonId'
                  or $attrName = 'ProfileId' or $attrName = 'ProjectOwnerId' or $attrName = 'PracticeCapabilityId' or $attrName = 'OpportunityTransitionId' or $attrName = 'PriorityId' 
                  or $attrName = 'ManagerId' or $attrName = 'SeniorityId' or $attrName = 'RecruiterId' or $attrName = 'TitleId' or $attrName = 'TitleTypeId' or $attrName = 'PricingListId' 
                  or $attrName = 'BusinessUnitId' or $attrName = 'BusinessGroupId'  or $attrName = 'CSATId' or $attrName = 'ReviewerId' or $attrName = 'SeniorManagerId' or $attrName = 'BusinessTypeId'"></xsl:when>
        <xsl:otherwise>
          <xsl:call-template name="FriendlyName">
            <xsl:with-param name="attrName" select="name()" />
          </xsl:call-template>:
          <b>
            <xsl:choose>
              <xsl:when test="$isNew">
                <xsl:call-template name="DisplayRedirect" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:call-template name="DisplayValue" />
              </xsl:otherwise>
            </xsl:choose>
          </b>
          <xsl:element name="br"></xsl:element>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <!-- Really displays a value -->
  <xsl:template name="DisplayValue" >
    <xsl:value-of select="."/>
  </xsl:template>

  <!-- Verifies whether a hyperlink should be displayed and generate it if necessary -->
  <xsl:template name="DisplayRedirect">
    <xsl:variable name="needHyperlink">
      <xsl:call-template name="CheckForHyperlink"></xsl:call-template>
    </xsl:variable>

    <xsl:choose>
      <xsl:when test="$needHyperlink = 'true'">
        <xsl:choose>
          <xsl:when test="(($rootName = 'Project' or $rootName = 'TimeEntry' or $rootName = 'ProjectCSAT') and (name() = 'ProjectId' or name() = 'Name' or name() = 'ProjectName' or name() = 'Project') 
                    and //DefaultProjectId = ./../@ProjectId) 
                    or ($rootName = 'Milestone' and (name() = 'MilestoneId' or name() = 'Name')
                        and //DefaultMileStoneId = ./../@MilestoneId)
                    or ($rootName = 'Milestone' and (name() = 'MilestoneProjectId' or name() = 'ProjectName')
                        and //DefaultProjectId = ./../@MilestoneProjectId )
                    or ($rootName = 'MilestonePerson' and (name() = 'MilestoneProjectId' or name() = 'ProjectName')
                        and //DefaultProjectId = ./../@MilestoneProjectId)
                    or (($rootName = 'MilestonePerson' or $rootName = 'TimeEntry') and (name() = 'MilestoneId' or name() = 'Name' or name() = 'Description')
                        and //DefaultMileStoneId =./../@MilestoneId)
                    or (($rootName = 'MilestonePerson' or $rootName = 'TimeEntry') and name() = 'MilestonePersonId'
                        and //DefaultMileStoneId = ./../@MilestoneId)
                    or ($rootName = 'Opportunity' and name() = 'Description')
                    or ($rootName = 'Project' and name() = 'Description')
                    or ($rootName = 'ProjectAttachment' and name() = 'ProjectName')
                    or ($rootName = 'TimeEntry' and name() = 'ProjectName' and ./../@IsAllowedToShow = 0)">
            <xsl:call-template name="DisplayValue" />
          </xsl:when>
          <xsl:otherwise>

            <a>
              <xsl:attribute name="href">
                <!-- Render an URL to navigate to the Project view -->
                <xsl:choose>
                  <xsl:when test="($rootName = 'Client' or $rootName = 'TimeEntry') and (name() = 'ClientId' or name() = 'ClientName')">
                    <xsl:text>ClientDetails.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@ClientId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="($rootName = 'Person' or $rootName = 'TimeEntry' or $rootName = 'Note' or $rootName = 'Roles' or $rootName = 'Practice'  or $rootName = 'ProjectCSAT' ) and (name() = 'PersonId' or name() = 'Name' or name() = 'ModifiedBy' or name() = 'ObjectName' or name() = 'ModifiedByName' or name() = 'ObjectPersonId' or name() = 'By' or name() = 'PracticeManagerId' or name() = 'PracticeManager' or name() = 'ReviewerId' or name() = 'Reviewer')">
                    <xsl:text>PersonDetail.aspx?id=</xsl:text>
                    <xsl:choose>
                      <xsl:when test="name() = 'ModifiedBy' or name() = 'ModifiedByName'">
                        <xsl:value-of select="./../@ModifiedBy"/>
                      </xsl:when>
                      <xsl:when test="name() = 'ObjectPersonId' or name() = 'ObjectName'">
                        <xsl:value-of select="./../@ObjectPersonId"/>
                      </xsl:when>
                      <xsl:when test="name() = 'PracticeManagerId' or name() = 'PracticeManager'">
                        <xsl:value-of select="./../@PracticeManagerId"/>
                      </xsl:when>
                      <xsl:when test="name() = 'ReviewerId' or name() = 'Reviewer'">
                        <xsl:value-of select="./../@ReviewerId"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="./../@PersonId"/>
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="($rootName = 'Project' or $rootName = 'TimeEntry' or $rootName = 'ProjectCSAT' ) and (name() = 'ProjectId' or name() = 'Name' or name() = 'ProjectName' or name() = 'Project')">
                    <xsl:text>ProjectDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@ProjectId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="($rootName = 'Milestone' or $rootName = 'TimeEntry') and (name() = 'MilestoneId' or name() = 'Name' or name() = 'Description')">
                    <xsl:text>MilestoneDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@MilestoneId"/>
                    <xsl:text>&amp;projectId=</xsl:text>
                    <xsl:value-of select="./../@MilestoneProjectId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="$rootName = 'ProjectAttachment' and (name() = 'ProjectId')">
                    <xsl:text>ProjectDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="//NEW_VALUES/@ProjectId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="$rootName = 'Opportunity' and (name() = 'OpportunityId' or name() = 'Name' )">
                    <xsl:text>OpportunityDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="//NEW_VALUES/@OpportunityId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="$rootName = 'Opportunity' and (name() = 'ClientId' or name() = 'ClientName' )">
                    <xsl:text>ClientDetails.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@ClientId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="$rootName = 'Milestone' and (name() = 'MilestoneProjectId' or name() = 'ProjectName')">
                    <xsl:text>ProjectDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@MilestoneProjectId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="$rootName = 'MilestonePerson' and (name() = 'MilestoneProjectId' or name() = 'ProjectName')">
                    <xsl:text>ProjectDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@MilestoneProjectId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="($rootName = 'MilestonePerson' or $rootName = 'TimeEntry') and (name() = 'MilestoneId' or name() = 'Name' or name() = 'Description')">
                    <xsl:text>MilestoneDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@MilestoneId"/>
                    <xsl:text>&amp;projectId=</xsl:text>
                    <xsl:choose>
                      <xsl:when test="$rootName = 'TimeEntry'">
                        <xsl:value-of select="./../@ProjectId"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="./../@MilestoneProjectId"/>
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:when test="($rootName = 'MilestonePerson' or $rootName = 'TimeEntry') and name() = 'MilestonePersonId'">
                    <xsl:text>MilestonePersonDetail.aspx?id=</xsl:text>
                    <xsl:value-of select="./../@MilestoneId"/>
                    <xsl:text>&amp;milestonePersonId=</xsl:text>
                    <xsl:value-of select="./../@MilestonePersonId"/>
                    <xsl:value-of select="$redirectUrl"/>
                  </xsl:when>
                  <xsl:otherwise>#</xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
              <xsl:call-template name="DisplayValue" />
            </a>

          </xsl:otherwise>
        </xsl:choose>

      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="DisplayValue" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="CheckForHyperlink">
    <xsl:choose>
      <xsl:when test="name() = 'ClientId' or name() = 'OpportunityId' or name() = 'ClientName' or name() = 'ModifiedByName' or name() = 'ModifiedBy' 
                or name() = 'ObjectName' or name() = 'ObjectPersonId' or name() = 'Description' or name() = 'PersonId' or name() = 'Name' or name() = 'ProjectId' 
                or name() = 'MilestoneId' or name() = 'ProjectName' or name() = 'Project' or name() = 'MilestoneProjectId' or name() = 'MilestonePersonId' or name() = 'By' or name() = 'PracticeManagerId' or name() = 'PracticeManager' or name() = 'ReviewerId' or name() = 'Reviewer'">
        <xsl:value-of select="true()"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="false()"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="DisplayChange">
    <xsl:param name="attrName" />
    <xsl:param name="oldValue" />
    <xsl:param name="newValue" />

    <xsl:call-template name="FriendlyName">
      <xsl:with-param name="attrName" select="$attrName" />
    </xsl:call-template>:
    <b>
      <xsl:choose>
        <xsl:when test="not($oldValue) or $oldValue = ''">NULL</xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$oldValue"/>
        </xsl:otherwise>
      </xsl:choose>
    </b>
    =&gt;
    <b>
      <xsl:choose>
        <xsl:when test="not($newValue) or $newValue = ''">NULL</xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$newValue"/>
        </xsl:otherwise>
      </xsl:choose>
    </b>
    <xsl:element name="br"></xsl:element>
  </xsl:template>

  <xsl:template name="FriendlyName">
    <xsl:param name="attrName" />

    <xsl:choose>
      <!-- Time Entry -->
      <xsl:when test="$attrName = 'CreateDate'">Entry Date</xsl:when>
      <xsl:when test="$attrName = 'EntryDate'">Entry Date</xsl:when>
      <xsl:when test="$attrName = 'ModifiedDate'">Modified Date</xsl:when>
      <xsl:when test="$attrName = 'ActualHours'">Actual Hours</xsl:when>
      <xsl:when test="$attrName = 'ForecastedHours'">Forecasted Hours</xsl:when>
      <xsl:when test="$attrName = 'TimeTypeId'">Work Type Id</xsl:when>
      <xsl:when test="$attrName = 'TimeTypeName'">Work Type</xsl:when>
      <xsl:when test="$attrName = 'ModifiedBy'">Modified By Id</xsl:when>
      <xsl:when test="$attrName = 'IsReviewed'">Is Reviewed</xsl:when>
      <xsl:when test="$attrName = 'ModifiedByName'">Modified By Name</xsl:when>
      <xsl:when test="$attrName = 'ModifiedByPersonId'">Modified By Id</xsl:when>
      <xsl:when test="$attrName = 'MilestoneDate'">Milestone Date</xsl:when>
      <xsl:when test="$attrName = 'ObjectName'">Person Name</xsl:when>
      <xsl:when test="$attrName = 'ObjectPersonId'">Person Id</xsl:when>
      <xsl:when test="($rootName = 'Opportunity') and ($attrName = 'Description')">Description</xsl:when>
      <xsl:when test="($rootName = 'Project') and ($attrName = 'Description')">Description</xsl:when>
      <xsl:when test="$attrName = 'Description'">Milestone Name</xsl:when>
      <xsl:when test="$attrName = 'ClientId'">Account Id</xsl:when>
      <xsl:when test="$attrName = 'FirstName'">First Name</xsl:when>
      <xsl:when test="$attrName = 'HashedPassword'">Password Reset</xsl:when>
      <xsl:when test="$attrName = 'LastName'">Last Name</xsl:when>
      <xsl:when test="$attrName = 'HireDate'">Hire Date</xsl:when>
      <xsl:when test="$attrName = 'PersonStatusName'">Status</xsl:when>
      <xsl:when test="$attrName = 'DefaultPractice'">Practice Area</xsl:when>
      <xsl:when test="$attrName = 'PTODaysPerAnnum'">PTO Days</xsl:when>
      <xsl:when test="$attrName = 'EmployeeNumber'">Employee Number</xsl:when>
      <xsl:when test="$attrName = 'PersonId'">Person ID</xsl:when>
      <xsl:when test="$attrName = 'ProjectId'">Project ID</xsl:when>
      <xsl:when test="$attrName = 'ClientName'">Account</xsl:when>
      <xsl:when test="$attrName = 'PracticeManagerId'">Practice Area Manager ID</xsl:when>
      <xsl:when test="$attrName = 'PracticeManagerFullName'">Practice Area Manager</xsl:when>
      <xsl:when test="$attrName = 'PracticeName'">Practice Area</xsl:when>
      <xsl:when test="$attrName = 'StartDate'">Start Date</xsl:when>
      <xsl:when test="$attrName = 'EndDate'">End Date</xsl:when>
      <xsl:when test="$attrName = 'ProjectStatusName'">Project Status</xsl:when>
      <xsl:when test="$attrName = 'ProjectNumber'">Project Number</xsl:when>
      <xsl:when test="$attrName = 'BuyerName'">Buyer Name</xsl:when>
      <xsl:when test="$attrName = 'MilestoneId'">Milestone ID</xsl:when>
      <xsl:when test="$attrName = 'MilestoneProjectId'">Project ID</xsl:when>
      <xsl:when test="$attrName = 'MilestonePersonId'">Milestone-Person ID</xsl:when>
      <xsl:when test="$attrName = 'FullName'">Person Name</xsl:when>
      <xsl:when test="$attrName = 'RoleName'">Role</xsl:when>
      <xsl:when test="$attrName = 'ProjectName'">Project Name</xsl:when>
      <xsl:when test="$attrName = 'HoursPerDay'">Hours Per Day</xsl:when>
      <xsl:when test="$attrName = 'IPAddress'">IP Address</xsl:when>
      <xsl:when test="$attrName = 'ExcMsg'">Exception message</xsl:when>
      <xsl:when test="$attrName = 'ExcSrc'">Exception source</xsl:when>
      <xsl:when test="$attrName = 'InnerExcMsg'">Inner Exception message</xsl:when>
      <xsl:when test="$attrName = 'InnerExcSrc'">Inner Exception source</xsl:when>
      <xsl:when test="$attrName = 'SourcePage'">Source Page Path</xsl:when>
      <xsl:when test="$attrName = 'SourceQuery'">Source Page Query</xsl:when>
      <xsl:when test="$attrName = 'SkillCategory'">Category</xsl:when>
      <xsl:when test="$attrName = 'SkillsDescription'">Skill</xsl:when>
      <xsl:when test="$attrName = 'SkillType'">Type</xsl:when>
      <xsl:when test="$attrName = 'SkillLevel'">Level</xsl:when>
      <xsl:when test="$attrName = 'IndustryDescription'">Industry</xsl:when>
      <xsl:when test="$attrName = 'IsDefaultManager'">Is Career Counselor</xsl:when>
      <xsl:when test="$attrName = 'ManagerName'">Career Counselor name</xsl:when>
      <xsl:when test="$attrName = 'ProjectGroupId'">Business Unit Id</xsl:when>
      <xsl:when test="$attrName = 'ProjectGroupName'">Business Unit</xsl:when>
      <xsl:when test="$attrName = 'Client'">Account</xsl:when>
      <xsl:when test="$attrName = 'TimeType'">Work Type</xsl:when>
      <xsl:when test="$attrName = 'Group'">Business Unit</xsl:when>
      <xsl:when test="$attrName = 'DivisionName'">Division</xsl:when>
      <xsl:when test="$attrName = 'IsNoteRequired'">Is Note Required</xsl:when>
      <xsl:when test="$attrName = 'AccountName'">Account Name</xsl:when>
      <xsl:when test="$attrName = 'AccountCode'">Account Code</xsl:when>
      <xsl:when test="$attrName = 'DefaultDiscount'">Default Discount</xsl:when>
      <xsl:when test="$attrName = 'DefaultTerms'">Default Terms</xsl:when>
      <xsl:when test="$attrName = 'IsActive'">Is Active</xsl:when>
      <xsl:when test="$attrName = 'IsChargeable'">Is Chargeable</xsl:when>
      <xsl:when test="$attrName = 'IsInternal'">Is Internal</xsl:when>
      <xsl:when test="$attrName = 'IsMarginColorInfoEnabled'">Is Margin Color Information Enabled</xsl:when>
      <xsl:when test="$attrName = 'DefaultDirector'">Default Director</xsl:when>
      <xsl:when test="$attrName = 'DefaultSalesperson'">Default Salesperson</xsl:when>
      <xsl:when test="$attrName = 'TerminationDate'">Termination Date</xsl:when>
      <xsl:when test="$attrName = 'TerminationReason'">Termination Reason</xsl:when>
      <xsl:when test="$attrName = 'IsDefault'">Is Default</xsl:when>
      <xsl:when test="$attrName = 'ProfileName'">Profile Name</xsl:when>
      <xsl:when test="$attrName = 'ProfileUrl'">Profile Url</xsl:when>
      <xsl:when test="$attrName = 'PictureUrl'">Picture Url</xsl:when>
      <xsl:when test="$attrName = 'IsCompanyInternal'">Is CompanyInternal</xsl:when>
      <xsl:when test="$attrName = 'IsNotesRequired'">Is NotesRequired</xsl:when>
      <xsl:when test="$attrName = 'PracticeManager'">Practice Area Owner</xsl:when>
      <xsl:when test="$attrName = 'ProjectOwner'">Project Owner</xsl:when>
      <xsl:when test="$attrName = 'SowBudget'">Sow Budget</xsl:when>
      <xsl:when test="$attrName = 'ProjectGroup'">Business Unit</xsl:when>
      <xsl:when test="$attrName = 'ClientDirector'">Client Director</xsl:when>
      <xsl:when test="$attrName = 'PracticeArea'">Practice Area</xsl:when>
      <xsl:when test="$attrName = 'ProjectStatus'">Project Status</xsl:when>
      <xsl:when test="$attrName = 'PracticeCapability'">Practice Capability</xsl:when>
      <xsl:when test="$attrName = 'ProjectedDeliveryDate'">Projected Delivery Date</xsl:when>
      <xsl:when test="$attrName = 'TransitionType'">Transition Type</xsl:when>
      <xsl:when test="$attrName = 'TransitionDate'">Transition Date</xsl:when>
      <xsl:when test="$attrName = 'NoteText'">Note Text</xsl:when>
      <xsl:when test="$attrName = 'OpportunityName'">Opportunity Name</xsl:when>
      <xsl:when test="$attrName = 'GroupName'">Group Name</xsl:when>
      <xsl:when test="$attrName = 'EstimatedRevenue'">Estimated Revenue</xsl:when>
      <xsl:when test="$attrName = 'OpportunityStatus'">Opportunity Status</xsl:when>
      <xsl:when test="$attrName = 'ProjectStartDate'">Project Start Date</xsl:when>
      <xsl:when test="$attrName = 'ProjectEndDate'">Project End Date</xsl:when>
      <xsl:when test="$attrName = 'CloseDate'">Close Date</xsl:when>
      <xsl:when test="$attrName = 'OpportunityNumber'">Opportunity Number</xsl:when>
      <xsl:when test="$attrName = 'FileName'">File Name</xsl:when>
      <xsl:when test="$attrName = 'UploadedDate'">Uploaded Date</xsl:when>
      <xsl:when test="$attrName = 'RecruiterName'">Recruiter Name</xsl:when>
      <xsl:when test="$attrName = 'TelephoneNumber'">Telephone Number</xsl:when>
      <xsl:when test="$attrName = 'TitleType'">Title Type</xsl:when>
      <xsl:when test="$attrName = 'PTOAccrual'">PTO Accrual</xsl:when>
      <xsl:when test="$attrName = 'MinimumSalary'">Salary Minimum</xsl:when>
      <xsl:when test="$attrName = 'MaximumSalary'">Salary Maximum</xsl:when>
      <xsl:when test="$attrName = 'SortOrder'">Sort Order</xsl:when>
      <xsl:when test="$attrName = 'BusinessGroup'">Business Group</xsl:when>
      <xsl:when test="$attrName = 'BusinessUnit'">Business Unit</xsl:when>
      <xsl:when test="$attrName = 'PricingList'">Pricing List</xsl:when>
      <xsl:when test="$attrName = 'ReviewStartDate'">Review Start Date</xsl:when>
      <xsl:when test="$attrName = 'ReviewEndDate'">Review End Date</xsl:when>
      <xsl:when test="$attrName = 'CompletionDate'">Completion Date</xsl:when>
      <xsl:when test="$attrName = 'ReferralScore'">Referral Score</xsl:when>
      <xsl:when test="$attrName = 'SeniorManager'">Senior Manager</xsl:when>
      <xsl:when test="$attrName = 'BusinessType'">Business Type</xsl:when>
      <xsl:when test="$attrName = 'Reviewer'">CSAT Owner</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$attrName"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>

