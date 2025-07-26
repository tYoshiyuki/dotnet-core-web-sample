
# ---------------------------------------------------------
# アプリケーションサーバを準備するステージ
# ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

RUN dotnet tool install -g dotnet-reportgenerator-globaltool
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY ["DotNetCoreWebSample.Web/DotNetCoreWebSample.Web.csproj", "DotNetCoreWebSample.Web/"]
COPY ["DotNetCoreWebSample.Web.Test/DotNetCoreWebSample.Web.Test.csproj", "DotNetCoreWebSample.Web.Test/"]

RUN dotnet restore "DotNetCoreWebSample.Web.Test/DotNetCoreWebSample.Web.Test.csproj"

COPY . .

RUN dotnet build "DotNetCoreWebSample.Web.Test/DotNetCoreWebSample.Web.Test.csproj" -c Debug --no-restore

# ---------------------------------------------------------
# テストを実行するステージ
# ---------------------------------------------------------
FROM build AS test

# テスト実行
RUN dotnet test "DotNetCoreWebSample.Web.Test/DotNetCoreWebSample.Web.Test.csproj" -c Debug --no-build --collect "XPlat Code Coverage" --logger "trx;logfilename=testResults.trx" --results-directory "/app/TestResults"

# カバレッジレポート出力
RUN reportgenerator -reports:"/app/TestResults/**/*.xml" -targetdir:"/app/CoverageReport" -reporttypes:HtmlInline_AzurePipelines_Light

# /app/TestResults/testResults.trx -> テスト結果
# /app/TestResults/{GUID}/coverage.cobertura.xml -> カバレッジファイル
# /app/CoverageReport -> カバレッジレポート
