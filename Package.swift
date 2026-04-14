// swift-tools-version: 6.3

import PackageDescription

let package = Package(
    name: "Sudoku",
    products: [.library(name: "Sudoku", targets: ["Sudoku"])],
    dependencies: [
        .package(
            url: "https://github.com/shipstone98/Utilities.git",
            from: "1.0.1"
        )
    ],
    targets: [
        .target(
            name: "Sudoku",
            dependencies: ["Utilities"]
        ),
        .testTarget(name: "SudokuTests", dependencies: ["Sudoku", "Utilities"])
    ],
    swiftLanguageModes: [.v6]
)
