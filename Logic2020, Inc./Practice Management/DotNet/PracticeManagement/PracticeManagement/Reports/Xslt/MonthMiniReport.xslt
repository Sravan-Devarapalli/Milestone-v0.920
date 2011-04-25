<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:ms="urn:schemas-microsoft-com:xslt">
    <xsl:output method="html" indent="no"/>

    <xsl:template match="Report">
		<table>
			<tr>
				<th align="center" colspan="2">
					<xsl:value-of select="ms:format-date(@Date, 'MMM yyyy')"/> - Monthly Report
				</th>
			</tr>
			<tr>
				<td nowrap="nowrap">Monthly Revenue</td>
				<td style="text-align: right">
					<xsl:call-template name="formatCurrency">
						<xsl:with-param name="value" select="@Revenue"/>
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap"># Employees</td>
				<td style="text-align: right">
					<xsl:value-of select="@EmployeesCount"/>
				</td>
			</tr>
			<tr title="All persons active at least part of this month, who are not in Admin or Offshore practice.">
				<td nowrap="nowrap"># Consultants</td>
				<td style="text-align: right">
					<xsl:value-of select="@ConsultantsCount"/>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">Rev/Employee</td>
				<td style="text-align: right">
					<xsl:call-template name="formatCurrency">
						<xsl:with-param name="value" select="number(@Revenue) div number(@EmployeesCount)"/>
					</xsl:call-template>
				</td>
			</tr>
		</table>
    </xsl:template>

	<xsl:template name="formatCurrency">
		<xsl:param name="value"/>

		$<xsl:choose>
			<xsl:when test="number($value) &gt;= 1000">
				<xsl:value-of select="format-number(number($value), '###,###,###,000')"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="format-number(number($value), '##0.00')"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>

