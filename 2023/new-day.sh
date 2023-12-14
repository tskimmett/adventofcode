dotnet new console -o $1
dotnet sln add $1/$1.csproj
cd $1
touch input.txt
touch sample.txt
