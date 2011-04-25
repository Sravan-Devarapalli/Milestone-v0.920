<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:template match="/*">    
    <xsl:apply-templates select="//NEW_VALUES" mode="list"></xsl:apply-templates>
  </xsl:template>
  <xsl:template match="NEW_VALUES" mode="list">    
    <xsl:value-of select="@By"/> - <xsl:value-of select="@NoteText"/>
  </xsl:template>  
</xsl:stylesheet>
