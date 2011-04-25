ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [PracticeManagement_log], FILENAME = '$(DefaultLogPath)$(DatabaseName).ldf', MAXSIZE = 674816 KB, FILEGROWTH = 10 %);


