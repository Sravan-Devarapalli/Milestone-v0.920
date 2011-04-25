<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt">
	<xsl:output method="html" indent="no"/>

    <xsl:param name="currentUrl" />

    <xsl:variable name="redirectUrl">&amp;returnTo=<xsl:value-of select="$currentUrl"/></xsl:variable>

  <xsl:template match="/*">
		<b>
			<xsl:value-of select="name()"/>
		</b>
	  <xsl:variable name="needHyperlink">
		<xsl:call-template name="CheckForHyperlink"></xsl:call-template>
	  </xsl:variable>
	&#160;
	  <xsl:choose>
		<xsl:when test="$needHyperlink = 'true'">
		  <a>
			<xsl:attribute name="href">
			  <!-- Render an URL to navigate to the Project view -->
			  <xsl:choose>
				<xsl:when test="name() = 'Person'">
				  <xsl:text>PersonDetail.aspx?id=</xsl:text>
				  <xsl:value-of select="//NEW_VALUES/@PersonId"/>
				  <xsl:value-of select="$redirectUrl"/>
				</xsl:when>
				<xsl:when test="name() = 'Project'">
				  <xsl:text>ProjectDetail.aspx?id=</xsl:text>
				  <xsl:value-of select="//NEW_VALUES/@ProjectId"/>
				  <xsl:value-of select="$redirectUrl"/>
				</xsl:when>
				<xsl:when test="name() = 'Milestone'">
				  <xsl:text>MilestoneDetail.aspx?id=</xsl:text>
				  <xsl:value-of select="//NEW_VALUES/@MilestoneId"/>
				  <xsl:text>&amp;projectId=</xsl:text>
				  <xsl:value-of select="//NEW_VALUES/@MilestoneProjectId"/>
				  <xsl:value-of select="$redirectUrl"/>
				</xsl:when>
				  <xsl:when test="name() = 'Opportunity' or name() = 'OpportunityTransition'">
					  <xsl:text>OpportunityDetail.aspx?id=</xsl:text>
					  <xsl:value-of select="//NEW_VALUES/@OpportunityId"/>
					  <xsl:value-of select="$redirectUrl"/>
				  </xsl:when>
				  <xsl:when test="name() = 'MilestonePerson'">
				  <xsl:text>MilestonePersonDetail.aspx?id=</xsl:text>
				  <xsl:value-of select="//NEW_VALUES/@MilestoneId"/>
				  <xsl:text>&amp;milestonePersonId=</xsl:text>
				  <xsl:value-of select="//NEW_VALUES/@MilestonePersonId"/>
				  <xsl:value-of select="$redirectUrl"/>
				</xsl:when>
				<xsl:otherwise>#</xsl:otherwise>
			  </xsl:choose>
			</xsl:attribute>
			<xsl:call-template name="DisplayValue" />
		  </a>
		</xsl:when>
		<xsl:otherwise>
		  <xsl:choose>
			<xsl:when test="name() = 'TimeEntry'">
			  <p>
			  D: <xsl:value-of select="//NEW_VALUES/@MilestoneDate"/><br/>
			  M: <xsl:value-of select="//NEW_VALUES/@Description"/><br/>
			  P: <xsl:value-of select="//NEW_VALUES/@ObjectName"/><br/>
			  Pr: <xsl:value-of select="//NEW_VALUES/@ProjectName"/><br/>
			  </p>
			</xsl:when>
			<xsl:otherwise>
			  <xsl:call-template name="DisplayValue" />
			</xsl:otherwise>
		  </xsl:choose>
		</xsl:otherwise>
	  </xsl:choose>
	</xsl:template>

  <xsl:template name="CheckForHyperlink">
	<xsl:choose>
	  <xsl:when test="name() = 'Project' or name() = 'Milestone' or name() = 'MilestonePerson' or name() = 'Person' or name() = 'Opportunity' or name() = 'OpportunityTransition'">
		<xsl:value-of select="true()"/>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:value-of select="false()"/>
	  </xsl:otherwise>
	</xsl:choose>
  </xsl:template>
  
  <!-- Really displays a value -->
  <xsl:template name="DisplayValue" >
	<xsl:choose>
	  <xsl:when test="name() = 'Person' and //NEW_VALUES/@FirstName">
		<!--<xsl:text>&#160;</xsl:text>-->
		<xsl:value-of select="//NEW_VALUES/@LastName"/>
		<xsl:text>,&#160;</xsl:text>
		<xsl:value-of select="//NEW_VALUES/@FirstName"/>
	  </xsl:when>
	  <xsl:when test="name() = 'TimeEntry'">
		  Time Entry
	  </xsl:when>
		<xsl:when test="name() = 'OpportunityTransition'">
			<xsl:value-of select="//NEW_VALUES/@OpportunityName"/>
		</xsl:when>
		<xsl:when test="name() = 'Export' and //NEW_VALUES/@User">
		<!--<xsl:text>&#160;</xsl:text>-->
		<xsl:value-of select="//NEW_VALUES/@User"/>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:value-of select="//NEW_VALUES/@Name"/>
	  </xsl:otherwise>
	</xsl:choose>
  </xsl:template>

</xsl:stylesheet>

