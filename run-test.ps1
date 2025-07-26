# テスト用Dockerイメージのビルド
docker build -t dotnet-core-web-sample/web/test . -f DotNetCoreWebSample.Web.Test/Dockerfile

# コンテナ起動 > 結果ファイル取得 > コンテナ停止
docker run -d -it --name dotnet-core-web-sample_web_test --rm dotnet-core-web-sample/web/test bash
docker cp dotnet-core-web-sample_web_test:/app/TestResults ./TestResults
docker cp dotnet-core-web-sample_web_test:/app/CoverageReport ./CoverageReport
docker stop dotnet-core-web-sample_web_test